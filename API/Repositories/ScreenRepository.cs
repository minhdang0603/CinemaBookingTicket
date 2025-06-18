using API.Data;
using API.Data.Models;
using API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class ScreenRepository : Repository<Screen>, IScreenRepository
    {
        private readonly ApplicationDbContext _context;

        public ScreenRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
