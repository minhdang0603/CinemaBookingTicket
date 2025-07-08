using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using Web.Models.DTOs.Response;
using Web.Models.ViewModels;
using Web.Services.IServices;

namespace Web.Areas.Public.Controllers
{
    [Area("Public")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IShowtimeService _showtimeService;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieService movieService, ILogger<MovieController> logger, IShowtimeService showtimeService)
        {
            _movieService = movieService;
            _logger = logger;
            _showtimeService = showtimeService;
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
                var filterDate = date ?? DateOnly.FromDateTime(DateTime.Today);

                // Lấy tất cả showtimes để có thông tin provinces và dates
                var allShowtimesResponse = await _showtimeService.GetShowTimesByMovieIdAsync<APIResponse>(id);
                var allShowtimes = new List<ShowTimeLiteDTO>();
                if (allShowtimesResponse != null && allShowtimesResponse.IsSuccess)
                {
                    allShowtimes = JsonConvert.DeserializeObject<List<ShowTimeLiteDTO>>(
                        Convert.ToString(allShowtimesResponse.Result) ?? string.Empty) ?? new List<ShowTimeLiteDTO>();
                }

                // Lấy showtimes đã filter
                var showtimeResponse = await _showtimeService.GetShowTimesByMovieIdAsync<APIResponse>(id, filterDate, provinceId);
                var showtimes = new List<ShowTimeLiteDTO>();
                if (showtimeResponse != null && showtimeResponse.IsSuccess)
                {
                    showtimes = JsonConvert.DeserializeObject<List<ShowTimeLiteDTO>>(
                        Convert.ToString(showtimeResponse.Result) ?? string.Empty) ?? new List<ShowTimeLiteDTO>();
                }

                // Create view model with movie data from API
                var viewModel = new MovieDetailViewModel
                {
                    Movie = movie,
                    ShowTimes = showtimes,
                    AllShowTimes = allShowtimes, // để lấy provinces và dates
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
    }
}