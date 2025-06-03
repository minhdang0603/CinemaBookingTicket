using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;

namespace API.Services;

public class MovieService : IMovieService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MovieService> _logger;

public MovieService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MovieService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }


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
