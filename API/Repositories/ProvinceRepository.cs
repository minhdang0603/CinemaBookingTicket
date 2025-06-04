using API.Data.Models;
using API.Repositories.IRepositories;

namespace API.Repositories
{
    public class ProvinceRepository : Repository<Province>, IProvinceRepository
    {
        private readonly ApplicationDbContext _context;

        public ProvinceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}