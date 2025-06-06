using API.Data.Models;
using API.Repositories.IRepositories;

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
