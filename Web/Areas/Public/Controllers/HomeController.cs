using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using Web.Models.DTOs.Response;
using Web.Services.IServices;
using Web.Models.DTOs.Request;

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
            try
            {
                // Load featured movies for carousel
                var featuredMovies = await GetFeaturedMoviesAsync();
                ViewBag.Movies = featuredMovies;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page data");
                // Return empty list if error occurs
                ViewBag.Movies = new List<MovieDTO>();
                return View();
            }
        }

        // API endpoint để load movies cho AJAX call
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

        private async Task<List<MovieDTO>> GetFeaturedMoviesAsync()
        {
            try
            {
                // Gọi API để lấy danh sách phim
                var movieResponse = await _movieService.GetAllMoviesAsync<APIResponse>();

                if (movieResponse == null || !movieResponse.IsSuccess)
                {
                    _logger.LogError("Failed to load movies from API: {Error}",
                        movieResponse?.ErrorMessages?.FirstOrDefault());
                    return new List<MovieDTO>();
                }

                // Deserialize response thành list MovieDTO
                var allMovies = JsonConvert.DeserializeObject<List<MovieDTO>>(
                    Convert.ToString(movieResponse.Result));

                if (allMovies == null || !allMovies.Any())
                {
                    _logger.LogWarning("No movies found from API");
                    return new List<MovieDTO>();
                }

                // Lọc những phim đang chiếu và lấy top 12 phim mới nhất
                var featuredMovies = allMovies
                    .Where(m => m.Status == "NowShowing") // Chỉ lấy phim đang chiếu
                    .OrderByDescending(m => m.ReleaseDate) // Sắp xếp theo ngày phát hành mới nhất
                    .Take(12) // Lấy 12 phim cho carousel (phù hợp với số dots)
                    .ToList();

                _logger.LogInformation("Successfully loaded {Count} featured movies", featuredMovies.Count);
                return featuredMovies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting featured movies");
                return new List<MovieDTO>();
            }
        }

        // Phương thức để lấy phim theo thể loại (nếu cần)
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