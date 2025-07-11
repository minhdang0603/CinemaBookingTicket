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

	public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<BookingService> logger, UserManager<ApplicationUser> userManager)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_httpContextAccessor = httpContextAccessor;
		_logger = logger;
		_userManager = userManager;
	}

	public async Task CancelBookingAsync(int bookingId)
	{
		// Check if user is authorized to cancel booking
		var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
					  ?? throw new AppException(ErrorCodes.UnauthorizedAccess());

		// Get booking by ID
		var booking = await _unitOfWork.Booking.GetAsync(b => b.Id == bookingId && b.ApplicationUser.Id == userId);

		if (booking == null || booking.ApplicationUser.Id != userId)
		{
			throw new AppException(ErrorCodes.EntityNotFound("Booking", bookingId));
		}

		booking.BookingStatus = Constant.Booking_Status_Cancelled;
		booking.IsActive = false;
		booking.LastUpdatedAt = DateTime.UtcNow;

		// Update booking
		await _unitOfWork.Booking.UpdateAsync(booking);

		// Commit transaction
		await _unitOfWork.SaveAsync();
	}

	//public async Task<string> CreateBookingWithPaymentAsync(BookingCreateDTO bookingCreateDTO)
	//{

	//    // 1. Tạo booking (code hiện tại của bạn)
	//    var booking = await CreateBookingAsync(bookingCreateDTO);

	//    VNPayRequestDTO request = new VNPayRequestDTO
	//    {
	//        BookingId = booking.Id,
	//        Amount = booking.TotalAmount,
	//        OrderInfo = $"Booking for ShowTime {booking.ShowTimeId} - User {booking.ApplicationUser.Name}",
	//        ClientIpAddress = "127.0.0.1"
	//    };

	//    // 2. Tạo payment và lấy URL redirect
	//    var paymentUrl = await _paymentService.CreateVNPayPaymentUrl(request);

	//    return paymentUrl; // Trả về URL cho controller

	//}

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

	public async Task<List<BookingDTO>> GetMyBookingsAsync()
	{
		// Get current user ID from claims
		var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
					  ?? throw new AppException(ErrorCodes.UnauthorizedAccess());

		// Get bookings for the current user
		var bookings = await _unitOfWork.Booking.GetAllAsync(
			b => b.ApplicationUser.Id == userId,
			includeProperties: "BookingDetails,ShowTime,ApplicationUser");

		var bookingDTOs = _mapper.Map<List<BookingDTO>>(bookings);
		bookingDTOs.ForEach(b =>
		{
			b.BookingItems = _mapper.Map<List<BookingDetailDTO>>(b.BookingItems);
		});
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