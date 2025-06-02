using API.Repositories.IRepositories;
using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repositories;

namespace API.Repositories
{
    public class ConcessionOrderRepository : Repository<ConcessionOrder>, IConcessionOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public ConcessionOrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
