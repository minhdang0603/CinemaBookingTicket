using API.Data.Models;
using API.Repositories.IRepositories;

namespace API.Repositories
{
    public class SeatRepository : Repository<Seat>, ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}