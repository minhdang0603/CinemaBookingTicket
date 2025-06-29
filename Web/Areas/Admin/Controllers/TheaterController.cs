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
    public class TheaterController : Controller
    {
        private readonly ITheaterService _theaterService;
        private readonly IProvinceService _provinceService;
        private readonly ILogger<TheaterController> _logger;
        private readonly IMapper _mapper;

        public TheaterController(ITheaterService theaterService, IMapper mapper, IProvinceService provinceService, ILogger<TheaterController> logger)
        {
            _theaterService = theaterService;
            _mapper = mapper;
            _provinceService = provinceService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Try to get data from API
            var theaterResponse = await _theaterService.GetAllTheatersAsync<APIResponse>();

            if (theaterResponse == null || !theaterResponse.IsSuccess)
            {
                _logger.LogError("Failed to load screens from API");
                TempData["error"] = theaterResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load screen list.";
                return View(new List<TheaterDTO>());
            }

            ViewBag.Provinces = await LoadProvinceDropdown();

            // Deserialize the response data into a list of TheaterDTO
            var theaters = JsonConvert.DeserializeObject<List<TheaterDTO>>(theaterResponse.Result.ToString());

            return View(theaters);
        }

        private async Task<IEnumerable<SelectListItem>> LoadProvinceDropdown()
        {
            // Try to get from API
            var provinceResponse = await _provinceService.GetAllProvincesAsync<APIResponse>();
            if (provinceResponse != null && provinceResponse.IsSuccess && provinceResponse.Result != null)
            {
                var provinces = JsonConvert.DeserializeObject<List<ProvinceDTO>>(provinceResponse.Result.ToString());
                return provinces.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                });
            }
            TempData["error"] = provinceResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load province list.";
            return new List<SelectListItem>();
        }

        public async Task<IActionResult> Create()
        {
            // Tạo TheaterCreateDTO mới và trả về Razor Page
            var model = new TheaterCreateDTO();

            // Load dữ liệu provinces vào ViewBag để dùng trong dropdown
            ViewBag.Provinces = await LoadProvinceDropdown();

            // Trả về view với model
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TheaterCreateDTO model)
        {
            _logger.LogInformation("Received theater creation request: {@model}", model);

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
                // Nếu ModelState không hợp lệ, trả về view với model và thông báo lỗi
                ViewBag.Provinces = await LoadProvinceDropdown();
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _theaterService.CreateTheaterAsync<APIResponse>(model, token);

            if (response != null && response.IsSuccess)
            {
                _logger.LogInformation("Theater created successfully: {Result}", response.Result);
                TempData["success"] = "Theater created successfully!";
                return RedirectToAction("Index");
            }

            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to create theater.";
            _logger.LogError("Failed to create theater: {ErrorMessages}", response?.ErrorMessages);

            ViewBag.Provinces = await LoadProvinceDropdown();
            return View(model);
        }

        // Phương thức xóa phòng chiếu
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Received request to delete theater with ID: {Id}", id);
            if (id <= 0)
            {
                TempData["error"] = "Invalid theater ID.";
                return Json(new { });
            }
            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _theaterService.DeleteTheaterAsync<APIResponse>(id, token);
            if (response != null && response.IsSuccess)
            {
                _logger.LogInformation("Theater deleted successfully: {Id}", id);
                TempData["success"] = "Theater deleted successfully!";
                return Json(new { });
            }
            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to delete theater.";
            _logger.LogError("Failed to delete theater: {ErrorMessages}", response?.ErrorMessages);
            return Json(new { });
        }

        // Phương thức hiển thị form sửa rạp chiếu phim
        public async Task<IActionResult> Edit(int id)
        {
            var theaterResponse = await _theaterService.GetTheaterByIdAsync<APIResponse>(id);

            if (theaterResponse == null || !theaterResponse.IsSuccess)
            {
                _logger.LogError("Failed to load theater with ID: {Id}", id);
                TempData["error"] = theaterResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load theater.";
                return RedirectToAction("Index");
            }

            var theater = JsonConvert.DeserializeObject<TheaterDetailDTO>(theaterResponse.Result?.ToString() ?? "{}");

            var model = _mapper.Map<TheaterUpdateDTO>(theater);

            // Load danh sách tỉnh/thành phố
            ViewBag.Provinces = await LoadProvinceDropdown();

            return View(model);
        }

        // Phương thức cập nhật rạp chiếu phim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TheaterUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                // Load danh sách tỉnh/thành phố lại nếu có lỗi
                ViewBag.Provinces = await LoadProvinceDropdown();
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _theaterService.UpdateTheaterAsync<APIResponse>(model.Id, model, token);

            if (response != null && response.IsSuccess)
            {
                _logger.LogInformation("Theater updated successfully: {Id}", model.Id);
                TempData["success"] = "Theater updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to update theater.";
            ViewBag.Provinces = await LoadProvinceDropdown();
            return View(model);
        }
    }
}