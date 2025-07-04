using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using Web.Models.DTOs.Response;
using Web.Services.IServices;
using Web.Models.DTOs.Request;
using Web.Models.ViewModels;
using Utility;

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
                // Load featured movies for the home page
                var featuredMovies = await GetFeaturedMoviesAsync();
                var model = new HomeIndexViewModel
                {
                    FeaturedMovies = featuredMovies
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page data");
                // Return empty model if error occurs
                var model = new HomeIndexViewModel();
                return View(model);
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
                // Gọi API endpoint mới để lấy phim cho trang chủ
                var movieResponse = await _movieService.GetMoviesForHomeAsync<APIResponse>();

                if (movieResponse == null || !movieResponse.IsSuccess)
                {
                    _logger.LogError("Failed to load movies for home from API: {Error}",
                        movieResponse?.ErrorMessages?.FirstOrDefault());
                    return new List<MovieDTO>();
                }

                // Deserialize response thành home movies model
                var responseData = Convert.ToString(movieResponse.Result);
                if (string.IsNullOrEmpty(responseData))
                {
                    _logger.LogWarning("Empty response data from GetMoviesForHome API");
                    return new List<MovieDTO>();
                }

                var homeMoviesData = JsonConvert.DeserializeObject<HomeMoviesDTO>(responseData);
                if (homeMoviesData == null)
                {
                    _logger.LogWarning("Failed to deserialize home movies data");
                    return new List<MovieDTO>();
                }

                // Lấy danh sách phim đang chiếu từ response
                var nowShowingMovies = homeMoviesData.NowShowing ?? new List<MovieDTO>();

                _logger.LogInformation("Successfully loaded {Count} featured movies for home", nowShowingMovies.Count);
                return nowShowingMovies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting featured movies for home");
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
                    Convert.ToString(movieResponse.Result) ?? "[]");

                var moviesByGenre = allMovies?
                    .Where(m => m.Genres != null && m.Genres.Any(g => g.Id == genreId))
                    .Where(m => m.Status == Constant.Movie_Status_NowShowing)
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