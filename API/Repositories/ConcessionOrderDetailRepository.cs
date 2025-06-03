using API.Data.Models;
using API.Repositories.IRepositories;

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
