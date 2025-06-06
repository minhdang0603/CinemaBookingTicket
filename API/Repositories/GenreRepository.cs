using API.Data.Models;
using API.Repositories.IRepositories;

namespace API.Repositories
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
