using API.Repositories.IRepositories;
using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repositories;

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
