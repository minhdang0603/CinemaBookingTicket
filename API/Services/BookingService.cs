using System.Security.Claims;
using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using Utility;

namespace API.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<BookingService> _logger;


    public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<BookingService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
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
            throw new AppException(ErrorCodes.BookingNotFound(bookingId));
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
            throw new AppException(ErrorCodes.BookingNotFound(bookingId));
        }

        if (booking.ApplicationUser.Id != userId && !_httpContextAccessor.HttpContext.User.IsInRole(Constant.Role_Admin))
        {
            throw new AppException(ErrorCodes.UnauthorizedAccess());
        }

		await _unitOfWork.Booking.RemoveAsync(booking);
        
        // Log the action
        _logger.LogInformation("Booking with ID {BookingId} has been deleted by user {UserId}.", bookingId, userId);
        
        // Save changes
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<BookingDTO>> GetAllBookingsAsync()
    {
        var bookings = await _unitOfWork.Booking.GetAllAsync(includeProperties: "BookingDetails,ShowTime,ApplicationUser");

        var bookingDTOs = _mapper.Map<List<BookingDTO>>(bookings);

        return bookingDTOs;
    }

    public async Task<BookingDTO> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _unitOfWork.Booking.GetAsync(b => b.Id == bookingId, includeProperties: "BookingDetails,ShowTime,ApplicationUser");

        if (booking == null)
        {
            throw new AppException(ErrorCodes.BookingNotFound(bookingId));
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
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                // Check seats availability
                foreach (var detail in bookingCreateDTO.BookingDetails)
                {
                    // Check seat exists
                    var seat = await _unitOfWork.Seat.GetAsync(s => s.Id == detail.SeatId);
                    if (seat == null)
                    {
                        throw new AppException(ErrorCodes.InternalServerError());
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
                        throw new AppException(ErrorCodes.InternalServerError());
                    }
                }

                // Map DTO to Booking entity
                var booking = _mapper.Map<Booking>(bookingCreateDTO);

                // Add booking to repository
                await _unitOfWork.Booking.CreateAsync(booking);

                await _unitOfWork.SaveAsync();

                var bookingDetails = _mapper.Map<List<BookingDetail>>(bookingCreateDTO.BookingDetails);

                // Set BookingId for each BookingDetail
                foreach (var detail in bookingDetails)
                {
                    detail.BookingId = booking.Id;
                    await _unitOfWork.BookingDetail.CreateAsync(detail);
                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
                return _mapper.Map<BookingDTO>(booking); // Return created booking
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating booking: {ex.Message}");
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}