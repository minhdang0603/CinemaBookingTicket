using API.Data.Models;
using API.Repositories.IRepositories;

namespace API.Repositories.IRepositories
{
    public interface IBookingRepository : IRepository<Booking>
    {

        Task<IEnumerable<Booking>> GetUserBookingHistoryAsync(string userId, int page = 1, int pageSize = 10);
        Task<Booking> GetBookingDetailAsync(int bookingId);
        Task<int> GetUserBookingCountAsync(string userId);
    }
}
