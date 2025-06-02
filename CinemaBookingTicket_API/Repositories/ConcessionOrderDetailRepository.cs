using API.Repositories.IRepositories;
using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repositories;

namespace API.Repositories
{
    public class ConcessionOrderDetailRepository : Repository<ConcessionOrderDetail>, IConcessionOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public ConcessionOrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}