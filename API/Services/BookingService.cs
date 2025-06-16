using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
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
                // Check seats availability
                foreach (var detail in bookingCreateDTO.BookingDetails)
                {
                    // Check seat exists
                    var seat = await _unitOfWork.Seat.GetAsync(s => s.Id == detail.SeatId);
                    if (seat == null)
                    {
                        throw new Exception($"Seat with ID {detail.SeatId} not found");
                    }

                    // Check seat not already booked for this showtime
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

                // Commit transaction
                await _unitOfWork.CommitAsync();

            }
            catch (Exception ex)
            {
                // Rollback transaction in case of error
                await _unitOfWork.RollbackAsync();
                throw new Exception("Error creating booking", ex);
            }
        }
    }

    public Task DeleteBookingAsync(int bookingId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BookingDTO> GetBookingByIdAsync(int bookingId)
    {
        throw new NotImplementedException();
    }
}