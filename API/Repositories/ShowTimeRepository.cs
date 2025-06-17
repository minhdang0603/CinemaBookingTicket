using API.Data;
using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Repositories.IRepositories;

namespace API.Repositories
{
    public class ShowTimeRepository : Repository<ShowTime>, IShowTimeRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowTimeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task AddRangeAsync(List<ShowTime> showTimes)
        {
            if (showTimes == null || showTimes.Count == 0)
            {
                throw new ArgumentNullException(nameof(showTimes), "ShowTimes cannot be null or empty");
            }

            _context.ShowTimes.AddRange(showTimes);
            return _context.SaveChangesAsync();
        }
    }
}
