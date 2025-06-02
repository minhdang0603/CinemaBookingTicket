using API.Repositories.IRepositories;
using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repositories;

namespace API.Repositories
{
    public class BookingDetailRepository : Repository<BookingDetail>, IBookingDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
