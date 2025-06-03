using API.Data.Models;
using API.Repositories.IRepositories;

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
