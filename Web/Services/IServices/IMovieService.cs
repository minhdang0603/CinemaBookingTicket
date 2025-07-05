using Web.Models.DTOs.Request;

namespace Web.Services.IServices
{
    public interface IMovieService
    {
        Task<T> GetAllMoviesAsync<T>();
        Task<T> GetMovieByIdAsync<T>(int movieId);
        Task<T> CreateMovieAsync<T>(MovieCreateDTO movie, string token);
        Task<T> UpdateMovieAsync<T>(MovieUpdateDTO movie, string token);
        Task<T> DeleteMovieAsync<T>(int movieId, string token);
        Task<T> GetMoviesForHomeAsync<T>();
        Task<T> GetShowtimesByMovieIdAsync<T>(int movieId);
    }
}
