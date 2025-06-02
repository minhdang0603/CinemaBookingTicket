using API.Repositories.IRepositories;
using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repositories;

namespace API.Repositories
{
    public class ConcessionCategoryRepository : Repository<ConcessionCategory>, IConcessionCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ConcessionCategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
