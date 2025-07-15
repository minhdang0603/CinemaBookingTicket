using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using Web.Models.DTOs.Response;
using Web.Models.ViewModels;
using Web.Services.IServices;
using Utility;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Areas.Public.Controllers
{
    [Area("Public")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IShowtimeService _showtimeService;
        private readonly IProvinceService _provinceService;
        private readonly IGenreService _genreService;
		private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieService movieService, ILogger<MovieController> logger, IShowtimeService showtimeService, IProvinceService provinceService, IGenreService genreService)
        {
            _movieService = movieService;
            _logger = logger;
            _showtimeService = showtimeService;
            _provinceService = provinceService;
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, DateOnly? date = null, int? provinceId = null)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid movie ID: {Id}", id);
                TempData["error"] = "ID phim không hợp lệ.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var movieResponse = await _movieService.GetMovieByIdAsync<APIResponse>(id);

                if (movieResponse == null || !movieResponse.IsSuccess)
                {
                    _logger.LogError("Failed to load movie details for ID {Id}: {Error}",
                        id, movieResponse?.ErrorMessages?.FirstOrDefault());
                    TempData["error"] = "Không thể tải thông tin phim.";
                    return RedirectToAction("Index", "Home");
                }

                var movie = JsonConvert.DeserializeObject<MovieDTO>(
                    Convert.ToString(movieResponse.Result) ?? string.Empty);

                if (movie == null)
                {
                    _logger.LogError("Movie not found for ID: {Id}", id);
                    TempData["error"] = "Không tìm thấy phim.";
                    return RedirectToAction("Index", "Home");
                }

                // Mặc định hiển thị lịch chiếu hôm nay nếu không có filter date
                var today = DateOnly.FromDateTime(DateTime.Today);
                var filterDate = date ?? today;

                // Lấy tất cả showtimes để có thông tin provinces và dates
                var allShowtimesResponse = await _showtimeService.GetShowTimesByMovieIdAsync<APIResponse>(id);
                var allShowtimes = new List<ShowTimeLiteDTO>();
                if (allShowtimesResponse != null && allShowtimesResponse.IsSuccess)
                {
                    allShowtimes = JsonConvert.DeserializeObject<List<ShowTimeLiteDTO>>(
                        Convert.ToString(allShowtimesResponse.Result) ?? string.Empty) ?? new List<ShowTimeLiteDTO>();

                    // Lọc chỉ lấy showtimes trong 5 ngày tới
                    var maxDate = today.AddDays(4);
                    allShowtimes = allShowtimes.Where(st => st.ShowDate >= today && st.ShowDate <= maxDate).ToList();
                }                // Lấy showtimes đã filter
                var showtimeResponse = await _showtimeService.GetShowTimesByMovieIdAsync<APIResponse>(id, filterDate, provinceId);
                var showtimes = new List<ShowTimeLiteDTO>();
                if (showtimeResponse != null && showtimeResponse.IsSuccess)
                {
                    showtimes = JsonConvert.DeserializeObject<List<ShowTimeLiteDTO>>(
                        Convert.ToString(showtimeResponse.Result) ?? string.Empty) ?? new List<ShowTimeLiteDTO>();

                    // Lọc chỉ lấy showtimes trong 5 ngày tới
                    var maxDate = today.AddDays(4);
                    showtimes = showtimes.Where(st => st.ShowDate >= today && st.ShowDate <= maxDate).ToList();
                }

                // Lấy tất cả provinces từ database (không chỉ từ showtimes)
                var allProvincesResponse = await _provinceService.GetAllProvincesAsync<APIResponse>();
                var allProvinces = new List<ProvinceDTO>();
                if (allProvincesResponse != null && allProvincesResponse.IsSuccess)
                {
                    allProvinces = JsonConvert.DeserializeObject<List<ProvinceDTO>>(
                        Convert.ToString(allProvincesResponse.Result) ?? string.Empty) ?? new List<ProvinceDTO>();
                }

                // Create view model with movie data from API
                var viewModel = new MovieDetailViewModel
                {
                    Movie = movie,
                    ShowTimes = showtimes,
                    AllShowTimes = allShowtimes, // để lấy provinces và dates
                    AllProvinces = allProvinces, // tất cả provinces từ database
                    SelectedDate = filterDate,
                    SelectedProvinceId = provinceId
                };

                return View(viewModel);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while loading movie details for ID: {Id}", id);
                TempData["error"] = "Đã xảy ra lỗi khi tải thông tin phim.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Route cho phim đang chiếu
        public async Task<IActionResult> MovieList(int pageNumber = 1, int pageSize = 8, int? genreId = null, int? year = null)
        {
            return await GetMovieList(pageNumber, pageSize, Constant.Movie_Status_NowShowing, "Phim Đang Chiếu", genreId, year);
        }

        // Route cho phim sắp chiếu
        public async Task<IActionResult> ComingSoon(int pageNumber = 1, int pageSize = 8, int? genreId = null, int? year = null)
        {
            return await GetMovieList(pageNumber, pageSize, Constant.Movie_Status_ComingSoon, "Phim Sắp Chiếu", genreId, year);
        }

        // Method chung để xử lý cả 2 loại phim với filter
        private async Task<IActionResult> GetMovieList(int pageNumber, int pageSize, string status, string pageTitle, int? genreId = null, int? year = null)
        {
            try
            {
                // Load genres for dropdowns
                ViewBag.Genres = await LoadGenreDropdown();
                ViewBag.Years = GetYears();

                // Load top 5 genres for popular section
                ViewBag.TopGenres = await LoadTopGenresAsync();

                // Set page title and status
                ViewBag.PageTitle = pageTitle;
                ViewBag.MovieStatus = status;
                ViewBag.IsComingSoon = status == Constant.Movie_Status_ComingSoon;

                // Set current filter values
                ViewBag.CurrentGenreId = genreId;
                ViewBag.CurrentYear = year;

                // Get all movies and apply filters
                var allMoviesResponse = await _movieService.GetAllMoviesAsync<APIResponse>();
                if (allMoviesResponse == null || !allMoviesResponse.IsSuccess)
                {
                    _logger.LogError("Failed to load movies from API");
                    TempData["error"] = allMoviesResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load movie list.";
                    return View("MovieList", new List<MovieDTO>());
                }

                var allMovies = JsonConvert.DeserializeObject<List<MovieDTO>>(Convert.ToString(allMoviesResponse.Result));

                // Filter by status first
                var movies = allMovies.Where(m => m.Status == status).ToList();
                _logger.LogInformation($"Movies after status filter ({status}): {movies.Count}");

                // Apply additional filters if needed
                if (genreId.HasValue)
                {
                    movies = movies.Where(m => m.Genres != null && m.Genres.Any(g => g.Id == genreId.Value)).ToList();
                    _logger.LogInformation($"Movies after genre filter (genreId={genreId}): {movies.Count}");
                }

                if (year.HasValue)
                {
                    movies = movies.Where(m => m.ReleaseDate.Year == year.Value).ToList();
                    _logger.LogInformation($"Movies after year filter (year={year}): {movies.Count}");
                }

                // Calculate pagination for filtered results
                var totalCount = movies.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var pagedMovies = movies.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                if (!pagedMovies.Any() && totalCount > 0)
                {
                    _logger.LogWarning("No movies found for the given page.");
                    TempData["info"] = "Không tìm thấy phim cho trang này.";
                }

                // Set pagination ViewBags
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalCount = totalCount;
                ViewBag.PageSize = pageSize;

                return View("MovieList", pagedMovies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the movie list.");
                TempData["error"] = "An unexpected error occurred. Please try again later.";
            }

            return View("MovieList", new List<MovieDTO>());
        }

        private async Task<IEnumerable<SelectListItem>> LoadGenreDropdown()
        {
            var genreResponse = await _genreService.GetAllGenresAsync<APIResponse>();
            if (genreResponse == null || !genreResponse.IsSuccess)
            {
                _logger.LogError("Failed to load genres from API");
                TempData["error"] = genreResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load genre list.";
                return new List<SelectListItem>();
            }
            // Deserialize the response data into a list of GenreDTO
            var genres = JsonConvert.DeserializeObject<List<GenreDTO>>(Convert.ToString(genreResponse.Result));
            return genres.Select(g => new SelectListItem
            {
                Text = g.Name,
                Value = g.Id.ToString()
            });
        }

        private List<SelectListItem> GetYears(int numberOfYears = 5)
        {
            var currentYear = DateTime.Now.Year;
            return Enumerable.Range(currentYear - (numberOfYears - 1), numberOfYears + 2).Select(year => new SelectListItem
            {
                Text = year.ToString(),
                Value = year.ToString()
            }).ToList();
        }

        // Thêm method mới để load top genres
        private async Task<IEnumerable<SelectListItem>> LoadTopGenresAsync()
        {
            try
            {
                var topGenresResponse = await _genreService.GetFiveTopGenresAsync<APIResponse>();
                if (topGenresResponse == null || !topGenresResponse.IsSuccess)
                {
                    _logger.LogError("Failed to load top genres from API");
                    return new List<SelectListItem>();
                }

                var topGenres = JsonConvert.DeserializeObject<List<GenreDTO>>(Convert.ToString(topGenresResponse.Result));
                return topGenres.Select(g => new SelectListItem
                {
                    Text = g.Name,
                    Value = g.Id.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading top genres.");
                return new List<SelectListItem>();
            }
        }
    }
}