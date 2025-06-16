using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IBookingService
{
    Task CreateBookingAsync(BookingCreateDTO bookingCreateDTO);
    Task<BookingDTO> GetBookingByIdAsync(int bookingId, bool? isActive = true);
    Task<List<BookingDTO>> GetAllBookingsAsync(bool? isActive = true);
    Task<List<BookingDTO>> GetAllBookingsWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true);
    Task DeleteBookingAsync(int bookingId);
}