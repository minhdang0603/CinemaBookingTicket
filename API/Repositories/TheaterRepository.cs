using API.Data.Models;
using API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class TheaterRepository : ITheaterRepository
    {
        private readonly ApplicationDbContext _context;

        public TheaterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Theater entity)
        {
            await _context.Theaters.AddAsync(entity);

        }

        public void Update(Theater entity)
        {
            _context.Theaters.Update(entity);
        }

        public void Remove(Theater entity)
        {
            _context.Theaters.Remove(entity);
        }

        public async Task<Theater?> GetAsync(int id) 
        {
            return await _context.Theaters.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
