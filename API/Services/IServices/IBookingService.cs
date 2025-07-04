using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IBookingService
{
    Task<BookingDTO> GetBookingByIdAsync(int bookingId);
    Task<List<BookingDTO>> GetAllBookingsAsync();
    Task<List<BookingDTO>> GetMyBookingsAsync();
    Task DeleteBookingAsync(int bookingId);
    Task CancelBookingAsync(int bookingId);
	Task<BookingDTO> CreateBookingAsync(BookingCreateDTO bookingCreateDTO);
}