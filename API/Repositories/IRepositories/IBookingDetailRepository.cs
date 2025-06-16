using API.Data.Models;
using API.Repositories.IRepositories;

namespace API.Repositories.IRepositories
{
    public interface IBookingDetailRepository : IRepository<BookingDetail>
    {
        Task<List<BookingDetail>> GetBookedSeatsAsync(int showtimeId, List<int> seatIds);
    }
}
