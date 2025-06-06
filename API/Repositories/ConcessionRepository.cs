using API.Data.Models;
using API.Repositories.IRepositories;

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
