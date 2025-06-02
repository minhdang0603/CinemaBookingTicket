using API.Repositories.IRepositories;
using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repositories;

namespace API.Repositories
{
    public class SeatTypeRepository : Repository<SeatType>, ISeatTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
