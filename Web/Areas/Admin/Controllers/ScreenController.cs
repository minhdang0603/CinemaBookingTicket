using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Models.ViewModels;
using Web.Services.IServices;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constant.Role_Admin)]
    public class ScreenController : Controller
    {
        private readonly IScreenService _screenService;
        private readonly ITheaterService _theaterService;
        private readonly ILogger<ScreenController> _logger;
        private readonly IMapper _mapper;

        public ScreenController(IScreenService screenService, IMapper mapper, ITheaterService theaterService, ILogger<ScreenController> logger)
        {
            _screenService = screenService;
            _mapper = mapper;
            _theaterService = theaterService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            // Try to get data from API
            var screenResponse = await _screenService.GetAllScreensAsync<APIResponse>();

            if (screenResponse == null || !screenResponse.IsSuccess)
            {
                _logger.LogError("Failed to load screens from API");
                TempData["error"] = screenResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load screen list.";
                return View(new List<ScreenDTO>());
            }

            ViewBag.Theaters = await LoadTheaterDropdown();

            // Deserialize the response data into a list of ScreenDTO
            var screens = JsonConvert.DeserializeObject<List<ScreenDTO>>(screenResponse.Result.ToString());

            return View(screens);
        }

        public async Task<IActionResult> Create()
        {
            // Tạo ScreenCreateDTO mới và trả về Razor Page
            var model = new ScreenCreateDTO();

            // Load dữ liệu theaters vào ViewBag để dùng trong dropdown

            ViewBag.Theaters = await LoadTheaterDropdown();

            // Trả về view với model
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScreenCreateDTO model)
        {
            _logger.LogInformation("Received screen creation request: {@model}", model);

            if (!ModelState.IsValid)
            {
                // Nếu ModelState không hợp lệ, trả về view với model và thông báo lỗi
                ViewBag.Theaters = await LoadTheaterDropdown();
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _screenService.CreateScreenAsync<APIResponse>(model, token);

            if (response != null && response.IsSuccess)
            {
                _logger.LogInformation("Screen created successfully: {Result}", response.Result);
                TempData["success"] = "Screen created successfully!";

                return RedirectToAction("Index");
            }

            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to create screen.";
            _logger.LogError("Failed to create screen: {ErrorMessages}", response?.ErrorMessages);

            ViewBag.Theaters = await LoadTheaterDropdown();
            return View(model);

        }

        // Phương thức hiển thị form sửa phòng chiếu
        public async Task<IActionResult> Edit(int id)
        {

            var screenResponse = await _screenService.GetScreenByIdAsync<APIResponse>(id);

            if (screenResponse == null || !screenResponse.IsSuccess)
            {
                _logger.LogError("Failed to load screen with ID: {Id}", id);
                TempData["error"] = screenResponse?.ErrorMessages?.FirstOrDefault();
                return RedirectToAction("Index");
            }

            var screen = JsonConvert.DeserializeObject<ScreenDetailDTO>(screenResponse.Result?.ToString() ?? "{}");

            var model = _mapper.Map<ScreenUpdateDTO>(screen);

            // Load danh sách rạp
            ViewBag.Theaters = await LoadTheaterDropdown();

            return View(model);
        }

        // Phương thức cập nhật phòng chiếu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ScreenUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                // Load danh sách rạp và loại ghế lại nếu có lỗi
                ViewBag.Theaters = await LoadTheaterDropdown();
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _screenService.UpdateScreenAsync<APIResponse>(model, token);

            if (response != null && response.IsSuccess)
            {
                _logger.LogInformation("Screen updated successfully: {Id}", model.Id);
                TempData["success"] = "Screen updated successfully!";

                return RedirectToAction("Index");
            }

            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to update screen.";
            ViewBag.Theaters = await LoadTheaterDropdown();
            return View(model);
        }

        // Phương thức lấy cấu hình ghế cho preview
        public async Task<IActionResult> GetSeatConfiguration(int id, int rows, int seatsPerRow)
        {
            _logger.LogInformation("Loading seat configuration for screen ID: {Id}, rows: {Rows}, seatsPerRow: {SeatsPerRow}",
                id, rows, seatsPerRow);

            // Lấy thông tin cấu hình ghế hiện tại từ API (nếu có)
            var seatResponse = await _screenService.GetSeatsByScreenIdAsync<APIResponse>(id);
            var seatConfiguration = new List<SeatDTO>();

            if (seatResponse != null && seatResponse.IsSuccess)
            {
                var seats = JsonConvert.DeserializeObject<List<SeatDTO>>(seatResponse.Result?.ToString() ?? "[]");
                if (seats != null && seats.Count > 0)
                {
                    seatConfiguration = seats;
                }
                else
                {
                    TempData["error"] = "No seats available for this screen.";
                }
            }
            else
            {
                TempData["error"] = seatResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to load seats.";
            }

            // Load danh sách loại ghế
            ViewBag.SeatTypes = await LoadSeatTypeList();

            // Tạo mô hình cho view
            var model = new SeatConfigurationViewModel
            {
                ScreenId = id,
                Rows = rows,
                SeatsPerRow = seatsPerRow,
                ExistingSeats = seatConfiguration
            };

            // Trả về partial view
            return PartialView("_SeatConfigurationPartial", model);
        }

        // Phương thức xóa phòng chiếu
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Received request to delete screen with ID: {Id}", id);
            if (id <= 0)
            {
                TempData["error"] = "Invalid screen ID.";
                return Json(new { });
            }
            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _screenService.DeleteScreenAsync<APIResponse>(id, token);
            if (response != null && response.IsSuccess)
            {
                _logger.LogInformation("Screen deleted successfully: {Id}", id);
                TempData["success"] = "Screen deleted successfully!";
                return Json(new { });
            }
            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to delete screen.";
            _logger.LogError("Failed to delete screen: {ErrorMessages}", response?.ErrorMessages);
            return Json(new { });
        }

        // Helper method to load seat types
        private async Task<List<SeatTypeDTO>> LoadSeatTypeList()
        {
            // Try to get seat types from API
            var seatTypeResponse = await _screenService.GetAllSeatTypesAsync<APIResponse>();
            if (seatTypeResponse != null && seatTypeResponse.IsSuccess && seatTypeResponse.Result != null)
            {
                var seatTypes = JsonConvert.DeserializeObject<List<SeatTypeDTO>>(seatTypeResponse.Result.ToString() ?? "[]");
                return seatTypes;
            }

            TempData["error"] = seatTypeResponse?.ErrorMessages?.First();
            return new List<SeatTypeDTO>();
        }

        // Helper method to load theaters for dropdown
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
    }
}
