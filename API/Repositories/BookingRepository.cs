using API.Data.Models;
using API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetUserBookingHistoryAsync(string userId, int page = 1, int pageSize = 10)
        {
            return await _context.Bookings
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Screen)
                    .ThenInclude(s => s.Theater)
                .Include(b => b.BookingDetails)
                .Include(b => b.Payment)
                .Include(b => b.ApplicationUser) 
                .Where(b => b.ApplicationUser.Id == userId && b.IsActive) 
                .OrderByDescending(b => b.BookingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUserBookingCountAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.ApplicationUser) 
                .CountAsync(b => b.ApplicationUser.Id == userId && b.IsActive); 
        }
        public async Task<Booking> GetBookingDetailAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Screen)
                    .ThenInclude(s => s.Theater)
                        .ThenInclude(t => t.Province)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Seat)
                    .ThenInclude(s => s.SeatType)
                .Include(b => b.Payment)
                .Include(b => b.ConcessionOrders)
                    .ThenInclude(co => co.ConcessionOrderDetails)
                    .ThenInclude(cod => cod.Concession)
                .Include(b => b.ApplicationUser) 
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.IsActive);
        }

        public async Task<bool> IsUserOwnerOfBookingAsync(int bookingId, string userId)
        {
            return await _context.Bookings
                .AnyAsync(b => b.Id == bookingId && b.ApplicationUser.Id == userId && b.IsActive);
        }
    }
}