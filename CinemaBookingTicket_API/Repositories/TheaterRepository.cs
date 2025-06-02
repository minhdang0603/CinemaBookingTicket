using API.Repositories.IRepositories;
using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repositories;

namespace API.Repositories
{
    public class TheaterRepository : Repository<Theater>, ITheaterRepository
    {
        private readonly ApplicationDbContext _context;

        public TheaterRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
