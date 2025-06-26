using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Services.IServices;
using static Utility.Constant;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constant.Role_Admin)]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MovieController> _logger;
        private readonly IGenreService _genreService;
        private readonly IMapper _map;
        public MovieController(IMovieService movieService, ILogger<MovieController> logger, IGenreService genreService, IMapper map)
        {
            _map = map;
            _movieService = movieService;
            _logger = logger;
            _genreService = genreService;
        }
        public async Task<IActionResult> Index()
        {
            var movieResponse = await _movieService.GetAllMoviesAsync<APIResponse>();
            if (movieResponse == null || !movieResponse.IsSuccess)
            {
                _logger.LogError("Failed to load movie from API");
                TempData["error"] = movieResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load screen list.";
                return View(new List<MovieDTO>());
            }
            // viewbag for genre dropdown
            ViewBag.Genres = await LoadGenreDropdown();
            // Deserialize the response data into a list of ScreenDTO
            var movies = JsonConvert.DeserializeObject<List<MovieDTO>>(Convert.ToString(movieResponse.Result));
            return View(movies);
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
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.AgeRatings = Enum.GetValues(typeof(AgeRatingType))
                                    .Cast<AgeRatingType>()
                                    .Select(e => new SelectListItem
                                    {
                                        Value = e.ToString(),
                                        Text = e.ToString()
                                    }).ToList();
            // viewbag for genre dropdown
            ViewBag.Genres = await LoadGenreDropdown();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieCreateDTO model)
        {
            _logger.LogInformation("Received movie creation request: {@model}", model);
            if (!ModelState.IsValid)
            {
                // Log validation errors
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning("Validation error for {Key}: {Error}",
                            state.Key, error.ErrorMessage);
                    }
                }
                ViewBag.AgeRatings = Enum.GetValues(typeof(AgeRatingType))
                                     .Cast<AgeRatingType>()
                                     .Select(e => new SelectListItem
                                     {
                                         Value = e.ToString(),
                                         Text = e.ToString()
                                     }).ToList();
                // viewbag for genre dropdown
                ViewBag.Genres = await LoadGenreDropdown();
                return View(model);
            }
            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _movieService.CreateMovieAsync<APIResponse>(model, token);
            if (response == null || !response.IsSuccess)
            {
                _logger.LogError("Failed to create movie: {Error}", response?.ErrorMessages?.FirstOrDefault());
                TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to create movie.";
                ViewBag.AgeRatings = Enum.GetValues(typeof(AgeRatingType))
                                    .Cast<AgeRatingType>()
                                    .Select(e => new SelectListItem
                                    {
                                        Value = e.ToString(),
                                        Text = e.ToString()
                                    }).ToList();
                // viewbag for genre dropdown
                ViewBag.Genres = await LoadGenreDropdown();
                return View(model);
            }
            _logger.LogInformation("Movie created successfully: {@model}", model);
            TempData["success"] = "Movie created successfully.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _movieService.GetMovieByIdAsync<APIResponse>(id);
            if (response == null || !response.IsSuccess)
            {
                _logger.LogError("Failed to load movie for editing: {Error}", response?.ErrorMessages?.FirstOrDefault());
                TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to load movie details.";
                return RedirectToAction(nameof(Index));
            }
            // Deserialize the response data into a MovieDtailDTO
            var movie = JsonConvert.DeserializeObject<MovieDetailDTO>(Convert.ToString(response.Result));
            if (movie == null)
            {
                _logger.LogError("Movie not found for ID: {Id}", id);
                TempData["error"] = "Movie not found.";
                return RedirectToAction(nameof(Index));
            }
            await SetViewBagsForMovieForm();
            // map to MovieUpdateDTO
            var movieUpdateDTO = _map.Map<MovieUpdateDTO>(movie);
            return View(movieUpdateDTO);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieUpdateDTO model)
        {
            _logger.LogInformation("Received movie update request: {@model}", model);

            if (!ModelState.IsValid)
            {
                // Log validation errors
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning("Validation error for {Key}: {Error}", state.Key, error.ErrorMessage);
                    }
                }

                await SetViewBagsForMovieForm(); // 🔄 Dùng lại
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _movieService.UpdateMovieAsync<APIResponse>(model, token);

            if (response == null || !response.IsSuccess)
            {
                _logger.LogError("Failed to update movie: {Error}", response?.ErrorMessages?.FirstOrDefault());
                TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to update movie.";

                await SetViewBagsForMovieForm(); // 🔄 Dùng lại
                return View(model);
            }

            _logger.LogInformation("Movie updated successfully: {@model}", model);
            TempData["success"] = "Movie updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Received request to delete movie with ID: {Id}", id);
            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _movieService.DeleteMovieAsync<APIResponse>(id, token);
            if (response == null || !response.IsSuccess)
            {
                _logger.LogError("Failed to delete movie: {Error}", response?.ErrorMessages?.FirstOrDefault());
                TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to delete movie.";
                return Json(new { success = false, message = TempData["error"] });
            }
            _logger.LogInformation("Movie deleted successfully with ID: {Id}", id);
            TempData["success"] = "Movie deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        private async Task SetViewBagsForMovieForm()
        {
            ViewBag.StatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = Constant.Movie_Status_ComingSoon, Text = "Coming Soon" },
                new SelectListItem { Value = Constant.Movie_Status_NowShowing, Text = "Now Showing" }
            };

            ViewBag.AgeRatings = Enum.GetValues(typeof(AgeRatingType))
                                     .Cast<AgeRatingType>()
                                     .Select(e => new SelectListItem
                                     {
                                         Value = e.ToString(),
                                         Text = e.ToString()
                                     }).ToList();

            ViewBag.Genres = await LoadGenreDropdown();
        }

        [HttpGet]
        public IActionResult Index123()
        {
            return View();
        }
    }
}
