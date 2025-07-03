using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using Web.Models.DTOs.Response;
using Web.Services.IServices;

namespace Web.Areas.Public.Controllers
{
    [Area("Public")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieService movieService, ILogger<MovieController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
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

                var movie = JsonConvert.DeserializeObject<MovieDetailDTO>(
                    Convert.ToString(movieResponse.Result));

                if (movie == null)
                {
                    _logger.LogError("Movie not found for ID: {Id}", id);
                    TempData["error"] = "Không tìm thấy phim.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Movie = movie;

                await LoadShowtimesForMovie(id);

                return View(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while loading movie details for ID: {Id}", id);
                TempData["error"] = "Đã xảy ra lỗi khi tải thông tin phim.";
                return RedirectToAction("Index", "Home");
            }
        }

        private async Task LoadShowtimesForMovie(int movieId)
        {
            try
            {
                ViewBag.Showtimes = GetSampleShowtimes();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load showtimes for movie {MovieId}", movieId);
                ViewBag.Showtimes = new List<object>();
            }
        }

        private List<object> GetSampleShowtimes()
        {
            return new List<object>
            {
                new {
                    CinemaName = "CGV Vincom Đồng Khởi",
                    Address = "Tầng 3, TTTM Vincom Center Đồng Khởi, 72 Lê Thánh Tôn & 45A Lý Tự Trọng, Quận 1",
                    Showtimes = new[] { "10:30", "13:15", "16:00", "18:45", "21:30" }
                },
                new {
                    CinemaName = "CGV Liberty Citypoint",
                    Address = "Tầng M - 1, Liberty Center Saigon Citypoint, 59 - 61 Pasteur, Q.1",
                    Showtimes = new[] { "11:00", "14:15", "17:30", "20:15" }
                },
                new {
                    CinemaName = "Galaxy Nguyễn Du",
                    Address = "116 Nguyễn Du, Q.1, Tp. Hồ Chí Minh",
                    Showtimes = new[] { "10:15", "13:00", "15:45", "18:30", "21:15" }
                }
            };
        }

        public IActionResult MovieBooking()
        {
            return View();
        }

        public IActionResult MovieList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMovieDetails(int id)
        {
            try
            {
                var movieResponse = await _movieService.GetMovieByIdAsync<APIResponse>(id);

                if (movieResponse == null || !movieResponse.IsSuccess)
                {
                    return Json(new { success = false, message = "Không thể tải thông tin phim" });
                }

                var movie = JsonConvert.DeserializeObject<MovieDetailDTO>(
                    Convert.ToString(movieResponse.Result));

                return Json(new { success = true, data = movie });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie details for ID: {Id}", id);
                return Json(new { success = false, message = "Đã xảy ra lỗi" });
            }
        }
    }
}