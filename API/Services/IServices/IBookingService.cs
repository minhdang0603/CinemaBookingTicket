using API.DTOs.Response;

namespace API.Services.IServices;

public interface IBookingService
{
    Task<IEnumerable<BookingHistoryDTO>> GetUserBookingHistoryAsync(string userId, int page = 1, int pageSize = 10);
    Task<BookingDetailDTO> GetBookingDetailAsync(int bookingId, string userId);
    Task<int> GetUserBookingCountAsync(string userId);
}