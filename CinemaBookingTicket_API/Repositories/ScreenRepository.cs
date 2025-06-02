using API.Repositories.IRepositories;
using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repositories;

namespace API.Repositories
{
    public class ScreenRepository : Repository<Screen>, IScreenRepository
    {
        private readonly ApplicationDbContext _context;

        public ScreenRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
