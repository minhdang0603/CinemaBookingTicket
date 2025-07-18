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
                var featuredMovies = await GetFeaturedMoviesAsync();

                // Debug log
                _logger.LogInformation("Index method - Featured movies count: {Count}", featuredMovies.Count);

                var viewModel = new HomeIndexViewModel
                {
                    FeaturedMovies = featuredMovies
                };
                ViewBag.Movies = viewModel.FeaturedMovies;

                // Debug ViewBag
                _logger.LogInformation("ViewBag.Movies count: {Count}",
                    ((List<MovieDTO>)ViewBag.Movies)?.Count ?? 0);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page data");
                var model = new HomeIndexViewModel();
                ViewBag.Movies = model.FeaturedMovies;
                return View(model);
            }
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

        // DEBUG ENDPOINT - Thêm method này để debug
        [HttpGet]
        public async Task<IActionResult> DebugApi()
        {
            try
            {
                _logger.LogInformation("=== DEBUG API START ===");

                var movieResponse = await _movieService.GetMoviesForHomeAsync<APIResponse>();

                var debugInfo = new
                {
                    ApiResponseNull = movieResponse == null,
                    IsSuccess = movieResponse?.IsSuccess ?? false,
                    HasResult = movieResponse?.Result != null,
                    ResultType = movieResponse?.Result?.GetType().Name,
                    RawResult = movieResponse?.Result?.ToString(),
                    ErrorMessages = movieResponse?.ErrorMessages,
                    ErrorCount = movieResponse?.ErrorMessages?.Count ?? 0
                };

                _logger.LogInformation("Debug Info: {@DebugInfo}", debugInfo);

                return Json(debugInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Debug API error");
                return Json(new
                {
                    Success = false,
                    Error = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        private async Task<List<MovieDTO>> GetFeaturedMoviesAsync()
        {
            try
            {
                _logger.LogInformation("=== GetFeaturedMoviesAsync START ===");

                var movieResponse = await _movieService.GetMoviesForHomeAsync<APIResponse>();

                _logger.LogInformation("API Call completed. Response null: {IsNull}", movieResponse == null);

                if (movieResponse == null)
                {
                    _logger.LogError("movieResponse is null");
                    return new List<MovieDTO>();
                }

                _logger.LogInformation("API Response IsSuccess: {IsSuccess}", movieResponse.IsSuccess);

                if (!movieResponse.IsSuccess)
                {
                    _logger.LogError("API call failed. Errors: {Errors}",
                        string.Join(", ", movieResponse.ErrorMessages ?? new List<string>()));
                    return new List<MovieDTO>();
                }

                var responseData = Convert.ToString(movieResponse.Result);
                _logger.LogInformation("Response data length: {Length}", responseData?.Length ?? 0);
                _logger.LogInformation("Response data preview: {Preview}",
                    responseData?.Substring(0, Math.Min(500, responseData.Length )));

                if (string.IsNullOrEmpty(responseData))
                {
                    _logger.LogWarning("Empty response data from GetMoviesForHome API");
                    return new List<MovieDTO>();
                }

                try
                {
                    var homeMoviesData = JsonConvert.DeserializeObject<HomeMoviesDTO>(responseData);

                    if (homeMoviesData == null)
                    {
                        _logger.LogWarning("HomeMoviesData is null after deserialization");
                        return new List<MovieDTO>();
                    }

                    _logger.LogInformation("Deserialization successful!");
                    _logger.LogInformation("NowShowing count: {Count}", homeMoviesData.NowShowing?.Count ?? 0);
                    _logger.LogInformation("ComingSoon count: {Count}", homeMoviesData.ComingSoon?.Count ?? 0);

                    var nowShowingMovies = homeMoviesData.NowShowing ?? new List<MovieDTO>();

                    if (nowShowingMovies.Any())
                    {
                        var firstMovie = nowShowingMovies.First();
                        _logger.LogInformation("First movie sample: ID={Id}, Title={Title}, Status={Status}",
                            firstMovie.Id, firstMovie.Title, firstMovie.Status);
                    }
                    else
                    {
                        _logger.LogWarning("NowShowing list is empty");
                    }

                    _logger.LogInformation("Returning {Count} movies", nowShowingMovies.Count);
                    return nowShowingMovies;
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "JSON Deserialization failed. Response: {Response}", responseData);

                    try
                    {
                        var directMovies = JsonConvert.DeserializeObject<List<MovieDTO>>(responseData);
                        _logger.LogInformation("Direct List<MovieDTO> deserialization successful: {Count} movies",
                            directMovies?.Count ?? 0);
                        return directMovies ?? new List<MovieDTO>();
                    }
                    catch (Exception directEx)
                    {
                        _logger.LogError(directEx, "Direct deserialization also failed");
                        return new List<MovieDTO>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetFeaturedMoviesAsync");
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}