using API.Data.Models;
using API.Repositories.IRepositories;

namespace API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IMovieRepository Movie { get; private set; }



        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Movie = new MovieRepository(_context);
        }


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
