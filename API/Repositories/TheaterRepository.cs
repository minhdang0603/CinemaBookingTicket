using API.Data;
using API.Data.Models;
using API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class TheaterRepository : Repository<Theater>, ITheaterRepository
    {
        private readonly ApplicationDbContext _context;

        public TheaterRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
