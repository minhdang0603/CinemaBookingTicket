using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using Utility;

namespace API.Services;

public class MovieService : IMovieService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MovieService> _logger;
    private readonly IShowTimeService _showTimeService;

    public MovieService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MovieService> logger, IShowTimeService showTimeService)
    {
        _showTimeService = showTimeService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }


    public async Task<MovieDTO> CreateMovieAsync(MovieCreateDTO movieCreateDTO)
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
        await _unitOfWork.SaveAsync();
        if (movieCreateDTO.GenreIds != null && movieCreateDTO.GenreIds.Any())
        {
            foreach (var genreId in movieCreateDTO.GenreIds)
            {
                var genreEntity = await _unitOfWork.Genre.GetAsync(g => g.Id == genreId);
                if (genreEntity == null)
                {
                    _logger.LogWarning($"Genre with ID {genreId} not found for movie {movie.Title}");
                    continue;
                }
                movie.MovieGenres.Add(new MovieGenre { MovieId = movie.Id, GenreId = genreEntity.Id });
            }
        }
        movie.CreatedAt = DateTime.Now;
        movie.LastUpdatedAt = DateTime.Now;

        await _unitOfWork.SaveAsync();
        _logger.LogInformation($"Movie {movie.Title} created successfully with ID {movie.Id}");

        return _mapper.Map<MovieDTO>(movie);
    }

    public async Task DeleteMovieAsync(int id)
    {
        var movie = await _unitOfWork.Movie.GetAsync(m => m.Id == id);
        if (movie == null)
        {
            _logger.LogError($"Movie with ID {id} not found");
            throw new AppException(ErrorCodes.EntityNotFound("Movie", id));
        }
        // Xóa bỏ các showtimes liên quan đến movie
        var showTimes = await _unitOfWork.ShowTime.GetAllAsync(st => st.MovieId == id);
        foreach (var showTime in showTimes)
        {
            await _showTimeService.DeleteShowTimeAsync(showTime.Id);
        }

        movie.IsActive = false;
        movie.LastUpdatedAt = DateTime.Now;
        await _unitOfWork.Movie.UpdateAsync(movie);
        await _unitOfWork.SaveAsync();
        _logger.LogInformation($"Movie {movie.Title} deleted successfully with ID {movie.Id}");
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

    public async Task<List<MovieDTO>> GetAllMoviesWithPaginationAsync(int pageNumber, int pageSize, string status, bool? isActive = true)
    {
        var movies = await _unitOfWork.Movie.GetAllAsync(
            m => m.IsActive == isActive && m.Status == status,
            includeProperties: "MovieGenres.Genre,ShowTimes",
            pageSize: pageSize,
            pageNumber: pageNumber);
        return _mapper.Map<List<MovieDTO>>(movies);
    }

    public async Task<MovieDTO> GetMovieByIdAsync(int id, bool? isActive = true)
    {
        var movie = await _unitOfWork.Movie.GetAsync(
            m => m.Id == id && m.IsActive == isActive,
            includeProperties: "MovieGenres.Genre,ShowTimes.Screen.Theater.Province");
        if (movie == null)
        {
            _logger.LogError($"Movie with ID {id} not found");
            throw new AppException(ErrorCodes.EntityNotFound("Movie", id));
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

    public async Task<MovieDTO> UpdateMovieAsync(int id, MovieUpdateDTO movieUpdateDTO)
    {
        var movie = await _unitOfWork.Movie.GetAsync(
            m => m.Id == id && m.IsActive == true,
            includeProperties: "MovieGenres"
        );
        if (movie == null)
        {
            _logger.LogError($"Movie with ID {id} not found");
            throw new AppException(ErrorCodes.EntityNotFound("Movie", id));
        }

        // Map các trường thông thường
        movie = _mapper.Map(movieUpdateDTO, movie);

        // Cập nhật lại MovieGenres
        if (movieUpdateDTO.GenreIds != null)
        {
            // Tìm các genre cần xóa
            var genresToRemove = movie.MovieGenres
                .Where(mg => !movieUpdateDTO.GenreIds.Contains(mg.GenreId))
                .ToList();

            foreach (var mg in genresToRemove)
            {
                movie.MovieGenres.Remove(mg);
            }

            // Thêm các genre mới chưa có
            foreach (var genreId in movieUpdateDTO.GenreIds)
            {
                if (!movie.MovieGenres.Any(mg => mg.GenreId == genreId))
                {
                    movie.MovieGenres.Add(new MovieGenre { MovieId = movie.Id, GenreId = genreId });
                }
            }
        }

        await _unitOfWork.Movie.UpdateAsync(movie);
        await _unitOfWork.SaveAsync();

        // Lấy lại movie đã update kèm genres
        var updatedMovie = await _unitOfWork.Movie.GetAsync(
            m => m.Id == movie.Id,
            includeProperties: "MovieGenres.Genre,ShowTimes"
        );

        _logger.LogInformation($"Movie {movie.Title} updated successfully with ID {movie.Id}");
        return _mapper.Map<MovieDTO>(updatedMovie);
    }

    public async Task<List<MovieDTO>> GetMoviesByStatusAsync(string status, int? limit = null, bool? isActive = true)
    {
        var movies = await _unitOfWork.Movie.GetAllAsync(
            m => m.Status == status && m.IsActive == isActive,
            includeProperties: "MovieGenres.Genre,ShowTimes");

        var result = _mapper.Map<List<MovieDTO>>(movies);

        // Áp dụng limit nếu được chỉ định
        if (limit.HasValue && limit.Value > 0)
        {
            result = result.Take(limit.Value).ToList();
        }

        return result;
    }

    public async Task<HomeMoviesDTO> GetMoviesForHomeAsync(int? nowShowingLimit = 12, int? comingSoonLimit = 6)
    {
        try
        {
            // Lấy phim đang chiếu
            var nowShowingMovies = await GetMoviesByStatusAsync(Constant.Movie_Status_NowShowing, nowShowingLimit);

            // Lấy phim sắp chiếu
            var comingSoonMovies = await GetMoviesByStatusAsync(Constant.Movie_Status_ComingSoon, comingSoonLimit);

            var result = new HomeMoviesDTO
            {
                NowShowing = nowShowingMovies,
                ComingSoon = comingSoonMovies
            };

            _logger.LogInformation("Successfully retrieved movies for home: {NowShowing} now showing, {ComingSoon} coming soon",
                nowShowingMovies.Count, comingSoonMovies.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting movies for home");
            throw;
        }
    }

    public async Task<List<ShowTimeLiteDTO>> GetShowTimesByMovieIdAsync(int movieId, DateTime? fromTime = null)
    {
        var from = fromTime ?? DateTime.Now.AddMinutes(30);
        var fromDate = DateOnly.FromDateTime(from);
        var fromTimeOnly = TimeOnly.FromDateTime(from);
        //var showTimes = await _unitOfWork.ShowTime.GetAllAsync(
        //    st => st.MovieId == movieId && st.IsActive == true &&
        //        (st.ShowDate > fromDate || (st.ShowDate == fromDate && st.StartTime > fromTimeOnly)),
        //    includeProperties: "Screen.Theater.Province"
        //);

        var showTimes = await _unitOfWork.ShowTime.GetAllAsync(
            st => st.MovieId == movieId && st.IsActive == true,
            includeProperties: "Screen.Theater.Province"
        );

        return _mapper.Map<List<ShowTimeLiteDTO>>(showTimes);
    }
}
