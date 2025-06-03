using CinemaBookingSystem.DTOs.Response;
using CinemaBookingTicket_API.DTOs.Request;

namespace CinemaBookingTicket_API.Services.IServices;

public interface IMovieService
{
    Task<List<MovieDTO>> GetAllMoviesAsync();
    Task<List<MovieDTO>> GetMoviesByGenreAsync(int genreId);
    Task<List<MovieDTO>> GetAllMoviesWithPaginationAsync(int pageNumber, int pageSize);
    Task<List<MovieDTO>> SearchMoviesAsync(string searchTerm);
    Task<MovieDTO> GetMovieByIdAsync(int id);
    Task CreateMovieAsync(MovieCreateDTO movieCreateDTO);
    Task UpdateMovieAsync(int id, MovieUpdateDTO movieUpdateDTO);
    Task DeleteMovieAsync(int id);
}
