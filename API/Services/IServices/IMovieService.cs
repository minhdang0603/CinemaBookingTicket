using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IMovieService
{
    Task<List<MovieDTO>> GetAllMoviesAsync(bool? isActive = true);
    Task<List<MovieDTO>> GetMoviesByGenreAsync(int genreId, bool? isActive = true);
    Task<List<MovieDTO>> GetAllMoviesWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true);
    Task<List<MovieDTO>> SearchMoviesAsync(string searchTerm, bool? isActive = true);
    Task<MovieDetailDTO> GetMovieByIdAsync(int id, bool? isActive = true);
    Task<MovieDTO> CreateMovieAsync(MovieCreateDTO movieCreateDTO);
    Task<MovieDTO> UpdateMovieAsync(int id, MovieUpdateDTO movieUpdateDTO);
    Task DeleteMovieAsync(int id);
}
