using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
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


    public async Task CreateMovieAsync(MovieCreateDTO movieCreateDTO)
    {
        if (movieCreateDTO == null)
        {
            _logger.LogError("MovieCreateDTO is null");
            throw new ArgumentNullException(nameof(movieCreateDTO));
        }

        if (await _unitOfWork.Movie.GetAsync(m => m.Title == movieCreateDTO.Title) != null)
        {
            _logger.LogError($"Movie with Title {movieCreateDTO.Title} already exists");
            throw new AppException(ErrorCodes.MovieAlreadyExists(movieCreateDTO.Title));
        }

        var movie = _mapper.Map<Movie>(movieCreateDTO);
        await _unitOfWork.Movie.CreateAsync(movie);

        if (movieCreateDTO.GenreIds != null && movieCreateDTO.GenreIds.Any())
        {
            foreach (var genreId in movieCreateDTO.GenreIds)
            {
                var genreEntity = await _unitOfWork.Genre.GetAsync(g => g.Id == genreId);
                if (genreEntity != null)
                {
                    movie.MovieGenres.Add(new MovieGenre { MovieId = movie.Id, GenreId = genreEntity.Id });
                }
            }
        }
        movie.CreatedAt = DateTime.Now;
        movie.LastUpdatedAt = DateTime.Now;

        await _unitOfWork.Movie.UpdateAsync(movie);
        _logger.LogInformation($"Movie {movie.Title} created successfully with ID {movie.Id}");

        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteMovieAsync(int id)
    {
        var movie = await _unitOfWork.Movie.GetAsync(m => m.Id == id);
        if (movie == null)
        {
            _logger.LogError($"Movie with ID {id} not found");
            throw new AppException(ErrorCodes.MovieNotFound(id));
        }
        movie.IsActive = false;
        movie.LastUpdatedAt = DateTime.Now;
        await _unitOfWork.Movie.UpdateAsync(movie);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<MovieDTO>> GetAllMoviesAsync(bool? isActive = true)
    {
        var movies = await _unitOfWork.Movie.GetAllAsync(
            m => m.IsActive == isActive,
            includeProperties: "MovieGenres.Genre,ShowTimes");
        if (movies == null || !movies.Any())
        {
            _logger.LogInformation("No movies found");
            return new List<MovieDTO>();
        }

        return _mapper.Map<List<MovieDTO>>(movies);
    }

    public async Task<List<MovieDTO>> GetAllMoviesWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
    {
        var movies = await _unitOfWork.Movie.GetAllAsync(
            m => m.IsActive == isActive,
            pageSize: pageSize,
            pageNumber: pageNumber,
            includeProperties: "MovieGenres.Genre,ShowTimes");
        return _mapper.Map<List<MovieDTO>>(movies);
    }

    public async Task<MovieDTO> GetMovieByIdAsync(int id, bool? isActive = true)
    {
        var movie = await _unitOfWork.Movie.GetAsync(
            m => m.Id == id && m.IsActive == isActive,
            includeProperties: "MovieGenres.Genre,ShowTimes");
        if (movie == null)
        {
            _logger.LogError($"Movie with ID {id} not found");
            throw new AppException(ErrorCodes.MovieNotFound(id));
        }
        return _mapper.Map<MovieDTO>(movie);
    }

    public async Task<List<MovieDTO>> GetMoviesByGenreAsync(int genreId, bool? isActive = true)
    {
        var movies = await _unitOfWork.Movie.GetAllAsync(
            m => m.MovieGenres.Any(mg => mg.GenreId == genreId) && m.IsActive == isActive,
            includeProperties: "MovieGenres.Genre,ShowTimes");
        return _mapper.Map<List<MovieDTO>>(movies);
    }

    public async Task<List<MovieDTO>> SearchMoviesAsync(string searchTerm, bool? isActive = true)
    {
        var movies = await _unitOfWork.Movie.GetAllAsync(
            m => m.Title.Contains(searchTerm) && m.IsActive == isActive,
            includeProperties: "MovieGenres.Genre,ShowTimes");
        return _mapper.Map<List<MovieDTO>>(movies);
    }

    public async Task UpdateMovieAsync(int id, MovieUpdateDTO movieUpdateDTO)
    {
        var movie = await _unitOfWork.Movie.GetAsync(m => m.Id == id && m.IsActive == true);
        if (movie == null)
        {
            _logger.LogError($"Movie with ID {id} not found");
            throw new AppException(ErrorCodes.MovieNotFound(id));
        }
        movie = _mapper.Map(movieUpdateDTO, movie);
        await _unitOfWork.Movie.UpdateAsync(movie);
        await _unitOfWork.SaveAsync();
    }
}
