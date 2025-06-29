using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Services.IServices;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constant.Role_Admin)]
    public class ShowtimeController : Controller
    {

        private readonly IShowtimeService _showtimeService;
        private readonly IMovieService _movieService;
        private readonly ITheaterService _theaterService;
        private readonly IScreenService _screenService;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowtimeController> _logger;

        public ShowtimeController(
            IShowtimeService showtimeService,
            ITheaterService theaterService,
            IScreenService screenService,
            IMovieService movieService,
            ILogger<ShowtimeController> logger,
            IMapper mapper
            )
        {
            _showtimeService = showtimeService;
            _movieService = movieService;
            _theaterService = theaterService;
            _screenService = screenService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: ShowtimeController
        public async Task<ActionResult> Index()
        {
            var showTimeResponse = await _showtimeService.GetAllShowTimesAsync<APIResponse>();

            if (showTimeResponse == null || !showTimeResponse.IsSuccess)
            {
                _logger.LogError("Failed to retrieve show times.");
                TempData["Error"] = showTimeResponse?.ErrorMessages?.First();
                return View(new List<ShowTimeDTO>());
            }

            ViewBag.Movies = await LoadMoviesDropDown();
            ViewBag.Theaters = await LoadTheaterDropdown();

            var showTimeList = JsonConvert.DeserializeObject<List<ShowTimeDTO>>(Convert.ToString(showTimeResponse.Result));

            return View(showTimeList);
        }

        public async Task<ActionResult> Create()
        {
            ViewBag.Movies = await LoadMoviesDropDown();
            ViewBag.Screens = await LoadScreenDropDown();
            return View(new ShowTimeCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ShowTimeCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Movies = await LoadMoviesDropDown();
                ViewBag.Screens = await LoadScreenDropDown();
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _showtimeService.AddShowTimeAsync<APIResponse>(model, token);

            if (response == null || !response.IsSuccess)
            {
                _logger.LogError("Failed to create show time.");
                TempData["Error"] = response?.ErrorMessages?.First();
                ViewBag.Movies = await LoadMoviesDropDown();
                ViewBag.Screens = await LoadScreenDropDown();
                return View(model);
            }

            TempData["Success"] = "Show time created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> CreateBulk()
        {
            ViewBag.Movies = await LoadMoviesDropDown();
            ViewBag.Screens = await LoadScreenDropDown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBulk([FromBody] List<ShowTimeCreateDTO> showtimes)
        {
            if (showtimes == null || !showtimes.Any())
            {
                return Json(new { isSuccess = false, errorMessages = new[] { "No showtimes provided." } });
            }

            _logger.LogInformation("Received bulk creation request for {Count} showtimes", showtimes.Count);

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _showtimeService.AddShowTimesAsync<APIResponse>(showtimes, token);

            if (response == null)
            {
                _logger.LogError("No response received from API");
                return Json(new { isSuccess = false, errorMessages = new[] { "No response from server. Please try again." } });
            }

			var result = JsonConvert.DeserializeObject<ShowTimeBulkResultDTO>(Convert.ToString(response.Result));

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
                _logger.LogInformation("All showtimes created successfully");

                return Json(new
                {
                    isSuccess = true,
                    message = "All showtimes created successfully"
                });
			} 
            else if(response.StatusCode == System.Net.HttpStatusCode.MultiStatus)
            {
                _logger.LogWarning("Some showtimes failed to create");
                return Json(new
                {
                    isSuccess = true,
                    message = $"Create {result.SuccessfulShowTimes.Count} showtimes successfully!",
                    errorMessage = result.FailedShowTimes
				});
			} 
            else
            {
                return Json(new
                {
                    isSuccess = false,
					errorMessage = response.ErrorMessages
				});
			}
        }

        public async Task<ActionResult> Edit(int id)
        {
            var response = await _showtimeService.GetShowTimeByIdAsync<APIResponse>(id);
            if (response == null || !response.IsSuccess)
            {
                _logger.LogError("Failed to retrieve show time for editing.");
                TempData["Error"] = response?.ErrorMessages?.First();
                return RedirectToAction(nameof(Index));
            }
            var showTime = JsonConvert.DeserializeObject<ShowTimeDTO>(Convert.ToString(response.Result));
            var showTimeUpdateDTO = _mapper.Map<ShowTimeUpdateDTO>(showTime);
            ViewBag.Movies = await LoadMoviesDropDown();
            ViewBag.Screens = await LoadScreenDropDown();
            return View(showTimeUpdateDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ShowTimeUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Movies = await LoadMoviesDropDown();
                ViewBag.Screens = await LoadScreenDropDown();
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _showtimeService.UpdateShowTimeAsync<APIResponse>(model.Id, model, token);

            if (response == null || !response.IsSuccess)
            {
                _logger.LogError("Failed to update show time.");
                TempData["Error"] = response?.ErrorMessages?.First();
                ViewBag.Movies = await LoadMoviesDropDown();
                ViewBag.Screens = await LoadScreenDropDown();
                return View(model);
            }

            TempData["Success"] = "Show time updated successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Received request to delete showtime with ID: {Id}", id);
            if (id <= 0)
            {
                TempData["error"] = "Invalid showtime ID.";
                return Json(new { });
            }
            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _showtimeService.DeleteShowTimeAsync<APIResponse>(id, token);
            if (response != null && response.IsSuccess)
            {
                _logger.LogInformation("Showtime deleted successfully: {Id}", id);
                TempData["success"] = "Showtime deleted successfully!";
                return Json(new { });
            }
            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to delete showtime.";
            _logger.LogError("Failed to delete showtime: {ErrorMessages}", response?.ErrorMessages);
            return Json(new { });
        }

        private async Task<IEnumerable<SelectListItem>> LoadMoviesDropDown()
        {
            var movieResponse = await _movieService.GetAllMoviesAsync<APIResponse>();

            if (movieResponse == null || !movieResponse.IsSuccess)
            {
                _logger.LogError("Failed to retrieve movies.");
                TempData["Error"] = movieResponse?.ErrorMessages?.First();
                return new List<SelectListItem>();
            }

            var movies = JsonConvert.DeserializeObject<List<MovieDTO>>(Convert.ToString(movieResponse.Result));
            return movies.Select(m => new SelectListItem
            {
                Text = m.Title,
                Value = m.Id.ToString()
            }).ToList();
        }

        private async Task<IEnumerable<SelectListItem>> LoadTheaterDropdown()
        {
            // Try to get theaters from API
            var theaterResponse = await _theaterService.GetAllTheatersAsync<APIResponse>();
            if (theaterResponse != null && theaterResponse.IsSuccess && theaterResponse.Result != null)
            {
                var theaters = JsonConvert.DeserializeObject<List<TheaterDTO>>(theaterResponse.Result.ToString());
                return theaters.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                });
            }
            TempData["error"] = theaterResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load theater list.";
            return new List<SelectListItem>();
        }

        private async Task<IEnumerable<SelectListItem>> LoadScreenDropDown()
        {
            // Try to get screens from API
            var screenResponse = await _screenService.GetAllScreensAsync<APIResponse>();
            if (screenResponse == null || !screenResponse.IsSuccess)
            {
                TempData["error"] = screenResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load screen list.";
                return new List<SelectListItem>();
            }

            var screens = JsonConvert.DeserializeObject<List<ScreenDTO>>(Convert.ToString(screenResponse.Result));
            return screens.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.Name} - {s.Theater.Name}"
            }).ToList();
        }
    }
}
