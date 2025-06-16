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

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task CreateBookingAsync(BookingCreateDTO bookingCreateDTO)
    {
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                // Kiểm tra xem ghế đã được đặt chưa
                foreach (var detail in bookingCreateDTO.BookingDetails)
                {
                    // Kiểm tra ghế có tồn tại không
                    var seat = await _unitOfWork.Seat.GetAsync(s => s.Id == detail.SeatId);
                    if (seat == null)
                    {
                        throw new Exception($"Seat with ID {detail.SeatId} not found");
                    }

                    // Kiểm tra ghế chưa được đặt cho showtime này
                    var existingBooking = await _unitOfWork.BookingDetail.GetAsync(
                        bd => bd.SeatId == detail.SeatId &&
                              bd.Booking.ShowTimeId == bookingCreateDTO.ShowTimeId &&
                              bd.Booking.BookingStatus != Constant.Booking_Status_Cancelled,
                        includeProperties: "Booking");

                    if (existingBooking != null)
                    {
                        throw new Exception($"Seat {detail.SeatName} is already booked");
                    }
                }

                // Add booking to repository
                var booking = _mapper.Map<Booking>(bookingCreateDTO);

                await _unitOfWork.Booking.CreateAsync(booking);

                await _unitOfWork.SaveAsync();

                var bookingDetails = _mapper.Map<List<BookingDetail>>(bookingCreateDTO.BookingDetails);

                // Đặt BookingId cho từng BookingDetail
                foreach (var detail in bookingDetails)
                {
                    detail.BookingId = booking.Id;
                    await _unitOfWork.BookingDetail.CreateAsync(detail);
                }

                await _unitOfWork.SaveAsync();

                // Commit transaction
                await _unitOfWork.CommitAsync();

            }
            catch (Exception ex)
            {
                // Rollback transaction trong trường hợp có lỗi
                await _unitOfWork.RollbackAsync();
                throw new Exception("Error creating booking", ex);
            }
        }
    }

    public async Task DeleteBookingAsync(int bookingId)
    {
        var booking = await _unitOfWork.Booking.GetAsync(b => b.Id == bookingId && b.IsActive == true, includeProperties: "BookingDetails");
        if (booking == null)
        {
            throw new AppException(ErrorCodes.BookingNotFound(bookingId));
        }

        // Mark booking as inactive
        booking.IsActive = false;
        await _unitOfWork.Booking.UpdateAsync(booking);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<BookingDTO>> GetAllBookingsAsync(bool? isActive = true)
    {
        var bookings = await _unitOfWork.Booking.GetAllAsync(b => b.IsActive == isActive, includeProperties: "BookingDetails");
        return _mapper.Map<List<BookingDTO>>(bookings);
    }

    public async Task<List<BookingDTO>> GetAllBookingsWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
    {
        var bookings = await _unitOfWork.Booking.GetAllAsync(
                b => b.IsActive == isActive,
                pageSize: pageSize,
                pageNumber: pageNumber,
                includeProperties: "BookingDetails");
        return _mapper.Map<List<BookingDTO>>(bookings);
    }

    public async Task<BookingDTO> GetBookingByIdAsync(int bookingId, bool? isActive = true)
    {
        var booking = await _unitOfWork.Booking.GetAsync(b => b.Id == bookingId && b.IsActive == isActive, includeProperties: "BookingDetails")
                ?? throw new AppException(ErrorCodes.BookingNotFound(bookingId));

        return _mapper.Map<BookingDTO>(booking);
    }
}