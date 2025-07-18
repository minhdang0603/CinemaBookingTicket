using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using Web.Models.DTOs.Response;
using Web.Models.ViewModels;
using Web.Services.IServices;

namespace Web.Areas.Public.Controllers
{
    [Area("Public")]
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IMovieService movieService, ILogger<HomeController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("HomeController Index method called");
            var viewModel = new HomeIndexViewModel();

            // Tạm thời dùng static data để test view
            viewModel.FeaturedMovies = GetSampleMovies();

            _logger.LogInformation("Using sample data with {Count} movies", viewModel.FeaturedMovies.Count);
            _logger.LogInformation("Debug info: {Info}", viewModel.GetDebugInfo());

            return View(viewModel);
        }

        private List<MovieDTO> GetSampleMovies()
        {
            return new List<MovieDTO>
            {
                new MovieDTO
                {
                    Id = 1,
                    Title = "Avengers: Endgame",
                    Director = "Anthony Russo, Joe Russo",
                    Cast = "Robert Downey Jr., Chris Evans, Mark Ruffalo, Chris Hemsworth",
                    Description = "Cuộc chiến cuối cùng của các siêu anh hùng Marvel",
                    Duration = 181,
                    Status = "NowShowing",
                    ReleaseDate = new DateOnly(2024, 6, 15),
                    AgeRating = "PG-13",
                    PosterUrl = "https://m.media-amazon.com/images/M/MV5BMTc5MDE2ODcwNV5BMl5BanBnXkFtZTgwMzI2NzQ2NzM@._V1_.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=TcMBFSGVi1c",
                    Genres = new List<GenreDTO>
                    {
                        new GenreDTO { Id = 1, Name = "Hành động", Description = "Phim hành động kịch tính" },
                        new GenreDTO { Id = 2, Name = "Khoa học viễn tưởng", Description = "Phim khoa học viễn tưởng" }
                    }
                },
                new MovieDTO
                {
                    Id = 2,
                    Title = "Spider-Man: No Way Home",
                    Director = "Jon Watts",
                    Cast = "Tom Holland, Zendaya, Benedict Cumberbatch",
                    Description = "Peter Parker phải đối mặt với các phản diện từ đa vũ trụ",
                    Duration = 148,
                    Status = "NowShowing",
                    ReleaseDate = new DateOnly(2024, 7, 1),
                    AgeRating = "PG-13",
                    PosterUrl = "https://m.media-amazon.com/images/M/MV5BZWMyYzFjYTYtNTRjYi00OGExLWE2YzgtOGRmYjAxZTU3NzBiXkEyXkFqcGdeQXVyMzQ0MzA0NTM@._V1_.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=JfVOs4VSpmA",
                    Genres = new List<GenreDTO>
                    {
                        new GenreDTO { Id = 1, Name = "Hành động", Description = "Phim hành động kịch tính" },
                        new GenreDTO { Id = 2, Name = "Khoa học viễn tưởng", Description = "Phim khoa học viễn tưởng" }
                    }
                },
                new MovieDTO
                {
                    Id = 3,
                    Title = "The Lion King",
                    Director = "Jon Favreau",
                    Cast = "Donald Glover, Beyoncé, James Earl Jones",
                    Description = "Câu chuyện về chú sư tử Simba và hành trình trở thành vua",
                    Duration = 118,
                    Status = "NowShowing",
                    ReleaseDate = new DateOnly(2024, 8, 10),
                    AgeRating = "G",
                    PosterUrl = "https://m.media-amazon.com/images/M/MV5BMjIwMjE1Nzc4NV5BMl5BanBnXkFtZTgwNDg4OTA1NzM@._V1_.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=7TavVZMewpY",
                    Genres = new List<GenreDTO>
                    {
                        new GenreDTO { Id = 6, Name = "Hoạt hình", Description = "Phim hoạt hình cho mọi lứa tuổi" }
                    }
                }
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetFeaturedMovies()
        {
            try
            {
                var movies = await GetFeaturedMoviesAsync();
                return Json(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured movies via AJAX");
                return Json(new List<MovieDTO>());
            }
        }

        // Debug endpoint
        [HttpGet]
        public IActionResult Debug()
        {
            var movies = GetSampleMovies();
            return Json(new
            {
                count = movies.Count,
                movies = movies.Select(m => new {
                    m.Id,
                    m.Title,
                    m.Duration,
                    m.Status,
                    m.AgeRating,
                    genreCount = m.Genres?.Count ?? 0,
                    genres = m.Genres?.Select(g => g.Name).ToArray(),
                    posterUrl = m.PosterUrl
                })
            });
        }

        private async Task<List<MovieDTO>> GetFeaturedMoviesAsync()
        {
            try
            {
                _logger.LogInformation("Starting to get featured movies from API");

                var movieResponse = await _movieService.GetAllMoviesAsync<APIResponse>();

                if (movieResponse == null || !movieResponse.IsSuccess)
                {
                    _logger.LogError("Failed to load movies from API: {Error}",
                        movieResponse?.ErrorMessages?.FirstOrDefault() ?? "Unknown error");
                    return new List<MovieDTO>();
                }

                _logger.LogInformation("API response received successfully");

                var allMovies = JsonConvert.DeserializeObject<List<MovieDTO>>(
                    Convert.ToString(movieResponse.Result));

                if (allMovies == null || !allMovies.Any())
                {
                    _logger.LogWarning("No movies found from API response");
                    return new List<MovieDTO>();
                }

                _logger.LogInformation("Found {Count} movies from API", allMovies.Count);

                var featuredMovies = allMovies
                    .Where(m => m.Status == "NowShowing") 
                    .OrderByDescending(m => m.ReleaseDate) 
                    .Take(12) 
                    .ToList();

                _logger.LogInformation("Filtered to {Count} featured movies", featuredMovies.Count);
                return featuredMovies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting featured movies");
                return new List<MovieDTO>();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMoviesByGenre(int genreId)
        {
            try
            {
                var movieResponse = await _movieService.GetAllMoviesAsync<APIResponse>();

                if (movieResponse == null || !movieResponse.IsSuccess)
                {
                    return Json(new List<MovieDTO>());
                }

                var allMovies = JsonConvert.DeserializeObject<List<MovieDTO>>(
                    Convert.ToString(movieResponse.Result));

                var moviesByGenre = allMovies?
                    .Where(m => m.Genres != null && m.Genres.Any(g => g.Id == genreId))
                    .Where(m => m.Status == "NowShowing")
                    .OrderByDescending(m => m.ReleaseDate)
                    .Take(12)
                    .ToList() ?? new List<MovieDTO>();

                return Json(moviesByGenre);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movies by genre {GenreId}", genreId);
                return Json(new List<MovieDTO>());
            }
        }

        // Error handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}