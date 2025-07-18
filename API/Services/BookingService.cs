using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Utility;

namespace API.Services;

public class BookingService : IBookingService
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly ILogger<BookingService> _logger;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IConcessionOrderService _concessionOrderService;
	private readonly IPaymentService _paymentService;

	public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<BookingService> logger, UserManager<ApplicationUser> userManager, IConcessionOrderService concessionOrderService, IPaymentService paymentService)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_httpContextAccessor = httpContextAccessor;
		_logger = logger;
		_userManager = userManager;
		_concessionOrderService = concessionOrderService;
		_paymentService = paymentService;
	}

	public async Task CancelBookingAsync(int bookingId)
	{
		// Check if user is authorized to cancel booking
		var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
					  ?? throw new AppException(ErrorCodes.UnauthorizedAccess());

		// Get booking by ID with all necessary related data
		var booking = await _unitOfWork.Booking.GetAsync(
			b => b.Id == bookingId,
			includeProperties: "ApplicationUser,ShowTime,Payment");

		if (booking == null)
		{
			throw new AppException(ErrorCodes.EntityNotFound("Booking", bookingId));
		}

		// Check if user is authorized to cancel this booking
		if (booking.ApplicationUser.Id != userId && !_httpContextAccessor.HttpContext.User.IsInRole(Constant.Role_Admin))
		{
			throw new AppException(ErrorCodes.UnauthorizedAccess());
		}

		// Check if booking is already cancelled
		if (booking.BookingStatus == Constant.Booking_Status_Cancelled || booking.BookingStatus == Constant.Booking_Status_Refunded)
		{
			throw new AppException(ErrorCodes.BookingAlreadyCancelled());
		}

		// User can only cancel booking 1 day before the show date
		var showTime = booking.ShowTime.ShowDate.ToDateTime(booking.ShowTime.StartTime);
		if (showTime.AddDays(-1) < DateTime.Now)
		{
			throw new AppException(ErrorCodes.BookingCancellationNotAllowed($"Vé chỉ có thể hủy trước {showTime.AddDays(-1).ToString("dd/MM/yyyy HH:mm")}"));
		}

		// Cancel all associated concession orders
		var concessionOrders = await _unitOfWork.ConcessionOrder.GetAllAsync(co => co.BookingId == bookingId && co.IsActive);
		foreach (var order in concessionOrders)
		{
			order.IsActive = false;
			order.OrderStatus = Constant.Booking_Status_Cancelled;
			order.LastUpdatedAt = DateTime.UtcNow;
			await _unitOfWork.ConcessionOrder.UpdateAsync(order);
		}

		// Update booking status
		booking.BookingStatus = Constant.Booking_Status_Cancelled;
		// Giữ nguyên IsActive = true để đảm bảo booking vẫn hiển thị trong lịch sử
		// booking.IsActive = false; // Bỏ dòng này để giữ booking hiển thị trong lịch sử
		booking.LastUpdatedAt = DateTime.UtcNow;
		await _unitOfWork.Booking.UpdateAsync(booking);

		// Use direct Payment navigation property instead of querying separately
		if (booking.Payment != null && booking.Payment.PaymentStatus == Constant.Payment_Status_Completed)
		{
			try
			{
				var refundRequest = new VNPayRefundRequestDTO
				{
					PaymentId = booking.Payment.Id,
					Amount = booking.Payment.Amount,
					OrderInfo = $"Hoàn tiền cho vé {booking.BookingCode}",
					TransactionDate = booking.Payment.PaymentDate,
					CreateBy = userId
				};

				var refundResult = await _paymentService.Refund(refundRequest);

				if (refundResult.Success)
				{
					// Vẫn giữ booking status là cancelled, chỉ cập nhật payment status là refunded
					booking.BookingStatus = Constant.Booking_Status_Cancelled;
					// Cập nhật trạng thái của Payment
					booking.Payment.PaymentStatus = Constant.Payment_Status_Refunded;
					booking.Payment.RefundAmount = booking.Payment.Amount;
					booking.Payment.RefundDate = DateTime.Now;
					booking.Payment.LastUpdatedAt = DateTime.Now;

					await _unitOfWork.Booking.UpdateAsync(booking);
					await _unitOfWork.Payment.UpdateAsync(booking.Payment);
					_logger.LogInformation("Booking {BookingId} has been cancelled and refunded successfully", bookingId);
				}
				else
				{
					_logger.LogWarning("Booking {BookingId} has been cancelled but refund failed: {Message}",
						bookingId, refundResult.Message);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing refund for booking {BookingId}", bookingId);
			}
		}
		else if (booking.Payment != null)
		{
			// Nếu payment không ở trạng thái completed, chỉ cập nhật status
			booking.Payment.PaymentStatus = Constant.Payment_Status_Failed;
			booking.Payment.LastUpdatedAt = DateTime.Now;
			await _unitOfWork.Payment.UpdateAsync(booking.Payment);
		}

		// Commit all changes
		await _unitOfWork.SaveAsync();
	}

	public async Task DeleteBookingAsync(int bookingId)
	{
		// Get current user ID from claims
		var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
				  ?? throw new AppException(ErrorCodes.UnauthorizedAccess());

		// Get booking by ID with appropriate includes to verify ownership
		var booking = await _unitOfWork.Booking.GetAsync(
			b => b.Id == bookingId && b.IsActive == true,
			includeProperties: "ApplicationUser");

		// Check if booking exists and belongs to the user
		if (booking == null)
		{
			throw new AppException(ErrorCodes.EntityNotFound("Booking", bookingId));
		}

		if (booking.ApplicationUser.Id != userId && !_httpContextAccessor.HttpContext.User.IsInRole(Constant.Role_Admin))
		{
			throw new AppException(ErrorCodes.UnauthorizedAccess());
		}

		// Find and delete all concession orders associated with the booking
		var concessionOrders = await _unitOfWork.ConcessionOrder.GetAllAsync(co => co.BookingId == bookingId,
			includeProperties: "ConcessionOrderDetails");

		foreach (var concessionOrder in concessionOrders)
		{
			// Delete all concession order details first
			foreach (var detail in concessionOrder.ConcessionOrderDetails.ToList())
			{
				await _unitOfWork.ConcessionOrderDetail.RemoveAsync(detail);
			}

			// Delete the concession order
			await _unitOfWork.ConcessionOrder.RemoveAsync(concessionOrder);
		}

		// Delete the booking
		await _unitOfWork.Booking.RemoveAsync(booking);

		// Log the action
		_logger.LogInformation("Booking with ID {BookingId} has been deleted by user {UserId}.", bookingId, userId);

		// Save changes
		await _unitOfWork.SaveAsync();
	}

	public async Task<List<BookingDTO>> GetAllBookingsAsync()
	{
		var bookings = await _unitOfWork.Booking.GetAllAsync(includeProperties: "BookingDetails,ShowTime,ShowTime.Movie,ShowTime.Screen.Theater,ApplicationUser");

		var bookingDTOs = _mapper.Map<List<BookingDTO>>(bookings);

		return bookingDTOs;
	}

	public async Task<BookingDTO> GetBookingByIdAsync(int bookingId)
	{
		var booking = await _unitOfWork.Booking.GetAsync(b => b.Id == bookingId, includeProperties: "BookingDetails,ShowTime,ShowTime.Movie,ShowTime.Screen.Theater,ApplicationUser");

		if (booking == null)
		{
			throw new AppException(ErrorCodes.EntityNotFound("Booking", bookingId));
		}

		// Kiểm tra quyền truy cập
		var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
					  ?? throw new AppException(ErrorCodes.UnauthorizedAccess());

		if (booking.ApplicationUser.Id != userId && !_httpContextAccessor.HttpContext.User.IsInRole(Constant.Role_Admin))
		{
			throw new AppException(ErrorCodes.UnauthorizedAccess());
		}

		var bookingDTO = _mapper.Map<BookingDTO>(booking);
		bookingDTO.BookingItems = _mapper.Map<List<BookingDetailDTO>>(bookingDTO.BookingItems);

		return bookingDTO;
	}

	public async Task<List<MyBookingDTO>> GetMyBookingsAsync()
	{
		var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
					  ?? throw new AppException(ErrorCodes.UnauthorizedAccess());
		var bookings = await _unitOfWork.Booking.GetAllAsync(
			b => b.ApplicationUser.Id == userId && b.IsActive,
			includeProperties: "BookingDetails,ShowTime,ShowTime.Movie,ShowTime.Screen.Theater,ApplicationUser");
		// total amount include concession orders
		var bookingDTOs = _mapper.Map<List<MyBookingDTO>>(bookings);
		foreach (var booking in bookingDTOs)
		{
			booking.BookingItems = _mapper.Map<List<BookingDetailDTO>>(booking.BookingItems);
			booking.ShowTime = _mapper.Map<MyShowTimeDTO>(booking.ShowTime);

			// Get concession orders for this booking
			var concessionOrders = await _concessionOrderService.GetConcessionOrdersByBookingIdAsync(booking.Id);
			booking.TotalAmount += concessionOrders.Sum(co => co.TotalAmount);
		}
		return bookingDTOs;
	}

	public async Task<BookingDTO> CreateBookingAsync(BookingCreateDTO bookingCreateDTO)
	{
		// Check seats availability
		foreach (var detail in bookingCreateDTO.BookingDetails)
		{
			// Check seat exists
			var seat = await _unitOfWork.Seat.GetAsync(s => s.Id == detail.SeatId);
			if (seat == null)
			{
				throw new AppException(ErrorCodes.SeatIsNotAvailable());
			}

			// Check seat not already booked for this showtime
			var existingBooking = await _unitOfWork.BookingDetail.GetAsync(
				bd => bd.SeatId == detail.SeatId &&
					  bd.Booking.ShowTimeId == bookingCreateDTO.ShowTimeId &&
					  bd.Booking.BookingStatus != Constant.Booking_Status_Cancelled,
				includeProperties: "Booking");

			if (existingBooking != null)
			{
				_logger.LogError($"Seat {detail.SeatName} is already booked");
				throw new AppException(ErrorCodes.SeatIsNotAvailable());
			}
		}

		// Map DTO to Booking entity
		var booking = _mapper.Map<Booking>(bookingCreateDTO);

		booking.ApplicationUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User)
			?? throw new AppException(ErrorCodes.UnauthorizedAccess());

		// Add booking to repository (BookingCode chưa có)
		await _unitOfWork.Booking.CreateAsync(booking);
		await _unitOfWork.SaveAsync();

		// Sau khi có Id, tạo BookingCode dạng CB000123
		booking.BookingCode = $"CB{booking.Id:D6}";
		await _unitOfWork.Booking.UpdateAsync(booking);
		await _unitOfWork.SaveAsync();

		booking.ShowTime = await _unitOfWork.ShowTime.GetAsync(
			st => st.Id == booking.ShowTimeId,
			includeProperties: "Movie,Screen.Theater");

		return _mapper.Map<BookingDTO>(booking);
	}

	// Method to cleanup expired bookings automatically
	public async Task<BookingDTO> UpdateBookingAsync(BookingUpdateDTO bookingUpdateDTO)
	{
		try
		{
			// Lấy thông tin người dùng hiện tại
			var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
						  ?? throw new AppException(ErrorCodes.UnauthorizedAccess());

			// Kiểm tra booking có tồn tại không và thuộc về người dùng hiện tại
			var booking = await _unitOfWork.Booking.GetAsync(
				b => b.Id == bookingUpdateDTO.BookingId && b.IsActive == true,
				includeProperties: "ApplicationUser,BookingDetails,ShowTime.Movie,ShowTime.Screen.Theater");

			if (booking == null)
			{
				throw new AppException(ErrorCodes.EntityNotFound("Booking", bookingUpdateDTO.BookingId));
			}

			if (booking.ApplicationUser.Id != userId && !_httpContextAccessor.HttpContext.User.IsInRole(Constant.Role_Admin))
			{
				throw new AppException(ErrorCodes.UnauthorizedAccess());
			}

			// Xóa các booking details hiện có
			foreach (var bookingDetail in booking.BookingDetails.ToList())
			{
				await _unitOfWork.BookingDetail.RemoveAsync(bookingDetail);
			}

			// Tổng giá trị của booking
			decimal totalAmount = 0;

			// Thêm booking details mới
			foreach (var detail in bookingUpdateDTO.BookingDetails)
			{
				// Kiểm tra ghế tồn tại
				var seat = await _unitOfWork.Seat.GetAsync(s => s.Id == detail.SeatId);
				if (seat == null)
				{
					throw new AppException(ErrorCodes.SeatIsNotAvailable());
				}

				// Kiểm tra ghế chưa được đặt cho suất chiếu này (loại trừ booking hiện tại)
				var existingBooking = await _unitOfWork.BookingDetail.GetAsync(
					bd => bd.SeatId == detail.SeatId &&
						  bd.Booking.ShowTimeId == bookingUpdateDTO.ShowTimeId &&
						  bd.Booking.Id != booking.Id &&
						  bd.Booking.BookingStatus != Constant.Booking_Status_Cancelled,
					includeProperties: "Booking");

				if (existingBooking != null)
				{
					_logger.LogError($"Seat {seat.SeatRow}{seat.SeatNumber} is already booked");
					throw new AppException(ErrorCodes.SeatIsNotAvailable());
				}

				// Thêm chi tiết đặt chỗ mới
				var bookingDetail = new BookingDetail
				{
					BookingId = booking.Id,
					SeatId = detail.SeatId,
					SeatPrice = detail.SeatPrice,
					SeatName = detail.SeatName,
					CreatedAt = DateTime.UtcNow,
					LastUpdatedAt = DateTime.UtcNow
				};

				await _unitOfWork.BookingDetail.CreateAsync(bookingDetail);
				totalAmount += detail.SeatPrice;
			}

			// Cập nhật thông tin booking
			booking.TotalAmount = totalAmount;
			booking.LastUpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Booking.UpdateAsync(booking);
			await _unitOfWork.SaveAsync();

			return _mapper.Map<BookingDTO>(booking);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Error updating booking {bookingUpdateDTO.BookingId}");
			throw;
		}
	}

	public async Task<int> CleanupExpiredBookingsAsync(int expiryMinutes)
	{
		try
		{
			_logger.LogInformation("Starting cleanup of expired bookings...");

			// Get all pending bookings created before the expiry time
			var expiredBookings = await _unitOfWork.Booking.GetAllAsync(
				b => b.BookingStatus == Constant.Booking_Status_Pending &&
					 b.CreatedAt.AddMinutes(expiryMinutes) < DateTime.Now &&
					 b.IsActive == true);

			if (!expiredBookings.Any())
			{
				_logger.LogInformation("No expired bookings found to clean up.");
				return 0;
			}

			int deletedCount = 0;

			// Delete each expired booking
			foreach (var booking in expiredBookings)
			{
				try
				{
					// Find and delete all concession orders associated with the booking
					var concessionOrders = await _unitOfWork.ConcessionOrder.GetAllAsync(
						co => co.BookingId == booking.Id,
						includeProperties: "ConcessionOrderDetails");

					foreach (var concessionOrder in concessionOrders)
					{
						// Delete all concession order details first
						foreach (var detail in concessionOrder.ConcessionOrderDetails.ToList())
						{
							await _unitOfWork.ConcessionOrderDetail.RemoveAsync(detail);
							_logger.LogInformation("Deleted concession order detail for order ID: {ConcessionOrderId}", concessionOrder.Id);
						}

						// Delete the concession order
						await _unitOfWork.ConcessionOrder.RemoveAsync(concessionOrder);
						_logger.LogInformation("Deleted concession order ID: {ConcessionOrderId} for booking ID: {BookingId}",
							concessionOrder.Id, booking.Id);
					}

					// Delete booking details if they exist
					var bookingDetails = await _unitOfWork.BookingDetail.GetAllAsync(bd => bd.BookingId == booking.Id);
					foreach (var detail in bookingDetails)
					{
						await _unitOfWork.BookingDetail.RemoveAsync(detail);
					}

					// Remove the booking
					await _unitOfWork.Booking.RemoveAsync(booking);
					deletedCount++;

					_logger.LogInformation("Deleted expired booking ID: {BookingId}", booking.Id);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error deleting expired booking ID: {BookingId}", booking.Id);
				}
			}

			// Save changes if any bookings were deleted
			if (deletedCount > 0)
			{
				await _unitOfWork.SaveAsync();
				_logger.LogInformation("Successfully deleted {Count} expired bookings", deletedCount);
			}

			return deletedCount;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in CleanupExpiredBookingsAsync");
			return 0;
		}
	}
}