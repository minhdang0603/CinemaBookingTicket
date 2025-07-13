using API.Data;
using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

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
        public async Task<List<ShowTime>> GetShowTimesByMovieIdAsync(int movieId, DateOnly? date, int? provinceId)
        {
            // get all showtimes for a specific movie
            if (movieId <= 0)
            {
                throw new ArgumentException("Invalid movie ID", nameof(movieId));
            }
            var showTimesQuery = _context.ShowTimes.Include(st => st.Screen).ThenInclude(x => x.Theater).ThenInclude(x => x.Province)
                .Where(st => st.MovieId == movieId && st.IsActive == true);
            // filter by date if provided
            if (date.HasValue)
            {
                showTimesQuery = showTimesQuery.Where(st => st.ShowDate == date.Value);
            }
            // check if provinceId is provided and exists in the database
            if (provinceId.HasValue && provinceId.Value > 0)
            {
                var provinceExists = await _context.Provinces.AnyAsync(p => p.Id == provinceId.Value && p.IsActive == true);
                if (!provinceExists)
                {
                    throw new ArgumentException("Invalid province ID", nameof(provinceId));
                }
                showTimesQuery = showTimesQuery.Where(st => st.Screen.Theater.ProvinceId == provinceId.Value);
            }
            return await showTimesQuery.ToListAsync();
        }
    }
}
