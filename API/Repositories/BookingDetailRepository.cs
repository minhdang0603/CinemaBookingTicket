using API.Data.Models;
using API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class BookingDetailRepository : Repository<BookingDetail>, IBookingDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<BookingDetail>> GetBookedSeatsAsync(int showtimeId, List<int> seatIds)
        {
            return _context.BookingDetails
                .Where(bd => bd.Booking.ShowTimeId == showtimeId && seatIds.Contains(bd.SeatId))
                .ToListAsync();
        }
    }
}