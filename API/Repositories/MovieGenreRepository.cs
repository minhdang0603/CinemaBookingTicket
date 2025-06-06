using API.Data.Models;
using API.Repositories.IRepositories;

namespace API.Repositories
{
    public class MovieGenreRepository : Repository<MovieGenre>, IMovieGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieGenreRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}