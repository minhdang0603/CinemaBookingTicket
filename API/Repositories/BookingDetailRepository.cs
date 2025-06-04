using API.Data.Models;
using API.Repositories.IRepositories;

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