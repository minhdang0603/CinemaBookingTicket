using API.Repositories.IRepositories;
using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repositories;

namespace API.Repositories
{
    public class ShowTimeRepository : Repository<ShowTime>, IShowTimeRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowTimeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
