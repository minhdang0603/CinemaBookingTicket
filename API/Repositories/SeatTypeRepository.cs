using API.Data.Models;
using API.Repositories.IRepositories;

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