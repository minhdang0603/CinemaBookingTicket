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
                    Convert.ToString(movieResponse.Result) ?? string.Empty);

                if (movie == null)
                {
                    _logger.LogError("Movie not found for ID: {Id}", id);
                    TempData["error"] = "Không tìm thấy phim.";
                    return RedirectToAction("Index", "Home");
                }

                // Create view model with movie data from API
                var viewModel = new MovieDetailViewModel
                {
                    Movie = movie
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
                    Convert.ToString(movieResponse.Result) ?? string.Empty);

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