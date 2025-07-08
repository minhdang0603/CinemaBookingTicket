using API.DTOs.Request;
using API.DTOs.Response;
using API.Data.Models;

namespace API.Repositories.IRepositories
{
    public interface IShowTimeRepository : IRepository<ShowTime>
    {
        Task AddRangeAsync(List<ShowTime> showTimes);
        Task<List<ShowTime>> GetShowTimesByMovieIdAsync(int movieId, DateOnly? date, int? provinceId);
    }
}
