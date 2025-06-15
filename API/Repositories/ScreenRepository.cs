using API.Data;
using API.Data.Models;
using API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class ScreenRepository : IScreenRepository
    {
        private readonly ApplicationDbContext _context;

        public ScreenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Screen entity)
        {
            await _context.Screens.AddAsync(entity);
        }

        public void Update(Screen entity)
        {
            _context.Screens.Update(entity);
        }

        public void Remove(Screen entity)
        {
            _context.Screens.Remove(entity);
        }

        public async Task<Screen?> GetAsync(int id)
        {
            return await _context.Screens
                .Include(s => s.Theater) // nếu bạn cần lấy luôn Theater
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
