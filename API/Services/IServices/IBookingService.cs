using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IBookingService
{
    Task CreateBookingAsync(BookingCreateDTO bookingCreateDTO);
    Task<BookingDTO> GetBookingByIdAsync(int bookingId);
    Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
    Task DeleteBookingAsync(int bookingId);
}