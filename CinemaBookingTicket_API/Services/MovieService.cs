using CinemaBookingSystem.DTOs.Response;
using CinemaBookingTicket_API.DTOs.Request;
using CinemaBookingTicket_API.Services.IServices;

namespace CinemaBookingTicket_API.Services;

public class MovieService : IMovieService
{



    public Task CreateMovieAsync(MovieCreateDTO movieCreateDTO)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMovieAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<MovieDTO>> GetAllMoviesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<MovieDTO>> GetAllMoviesWithPaginationAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<MovieDTO> GetMovieByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<MovieDTO>> GetMoviesByGenreAsync(int genreId)
    {
        throw new NotImplementedException();
    }

    public Task<List<MovieDTO>> SearchMoviesAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }

    public Task UpdateMovieAsync(int id, MovieUpdateDTO movieUpdateDTO)
    {
        throw new NotImplementedException();
    }
}
