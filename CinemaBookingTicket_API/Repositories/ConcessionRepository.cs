using API.Repositories.IRepositories;
using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repositories;

namespace API.Repositories
{
    public class ConcessionRepository : Repository<Concession>, IConcessionRepository
    {
        private readonly ApplicationDbContext _context;

        public ConcessionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
