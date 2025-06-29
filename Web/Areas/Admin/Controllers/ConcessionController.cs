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

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constant.Role_Admin)]
    public class ConcessionController : Controller
    {
        private readonly IConcessionService _concessionService;
        private readonly IConcessionCategoryService _concessionCategoryService;
        private readonly ILogger<ConcessionController> _logger;
        private readonly IMapper _mapper;

        public ConcessionController(IConcessionService concessionService, IMapper mapper, IConcessionCategoryService concessionCategoryService, ILogger<ConcessionController> logger)
        {
            _concessionService = concessionService;
            _mapper = mapper;
            _concessionCategoryService = concessionCategoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Try to get data from API
            var concessionResponse = await _concessionService.GetAllConcessionsAsync<APIResponse>();

            if (concessionResponse == null || !concessionResponse.IsSuccess)
            {
                _logger.LogError("Failed to load concessions from API");
                TempData["error"] = concessionResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load concession list.";
                return View(new List<ConcessionDTO>());
            }

            ViewBag.Categories = await LoadCategoryDropdown();

            // Deserialize the response data into a list of ConcessionDTO
            var concessions = JsonConvert.DeserializeObject<List<ConcessionDTO>>(concessionResponse.Result.ToString());

            return View(concessions);
        }

        public async Task<IActionResult> Create()
        {
            // Tạo ConcessionCreateDTO mới và trả về Razor Page
            var model = new ConcessionCreateDTO();

            // Load dữ liệu categories vào ViewBag để dùng trong dropdown
            ViewBag.Categories = await LoadCategoryDropdown();

            // Trả về view với model
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConcessionCreateDTO model)
        {
            _logger.LogInformation("Received concession creation request: {@model}", model);

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
                ViewBag.Categories = await LoadCategoryDropdown();
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _concessionService.CreateConcessionAsync<APIResponse>(model, token);

            if (response != null && response.IsSuccess)
            {
                _logger.LogInformation("Concession created successfully: {Result}", response.Result);
                TempData["success"] = "Concession created successfully!";

                return RedirectToAction("Index");
            }

            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to create concession.";
            _logger.LogError("Failed to create concession: {ErrorMessages}", response?.ErrorMessages);

            ViewBag.Categories = await LoadCategoryDropdown();
            return View(model);
        }

        // Phương thức hiển thị form sửa concession
        public async Task<IActionResult> Edit(int id)
        {
            var concessionResponse = await _concessionService.GetConcessionByIdAsync<APIResponse>(id);

            if (concessionResponse == null || !concessionResponse.IsSuccess)
            {
                _logger.LogError("Failed to load concession with ID: {Id}", id);
                TempData["error"] = concessionResponse?.ErrorMessages?.FirstOrDefault();
                return RedirectToAction("Index");
            }

            var concession = JsonConvert.DeserializeObject<ConcessionDTO>(concessionResponse.Result?.ToString() ?? "{}");

            var model = _mapper.Map<ConcessionUpdateDTO>(concession);

            // Load danh sách categories
            ViewBag.Categories = await LoadCategoryDropdown();

            return View(model);
        }

        // Phương thức cập nhật concession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConcessionUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                // Load danh sách categories lại nếu có lỗi
                ViewBag.Categories = await LoadCategoryDropdown();
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _concessionService.UpdateConcessionAsync<APIResponse>(model, token);

            if (response != null && response.IsSuccess)
            {
                _logger.LogInformation("Concession updated successfully: {Id}", model.Id);
                TempData["success"] = "Concession updated successfully!";

                return RedirectToAction("Index");
            }

            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to update concession.";
            ViewBag.Categories = await LoadCategoryDropdown();
            return View(model);
        }

        // Phương thức xóa concession
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Received request to delete concession with ID: {Id}", id);
            if (id <= 0)
            {
                TempData["error"] = "Invalid concession ID.";
                return Json(new { });
            }
            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _concessionService.DeleteConcessionAsync<APIResponse>(id, token);
            if (response != null && response.IsSuccess)
            {
                _logger.LogInformation("Concession deleted successfully: {Id}", id);
                TempData["success"] = "Concession deleted successfully!";
                return Json(new { });
            }
            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to delete concession.";
            _logger.LogError("Failed to delete concession: {ErrorMessages}", response?.ErrorMessages);
            return Json(new { });
        }

        // Helper method to load categories for dropdown
        private async Task<IEnumerable<SelectListItem>> LoadCategoryDropdown()
        {
            // Try to get categories from API
            var categoryResponse = await _concessionCategoryService.GetAllConcessionCategoriesAsync<APIResponse>();
            if (categoryResponse != null && categoryResponse.IsSuccess && categoryResponse.Result != null)
            {
                var categories = JsonConvert.DeserializeObject<List<ConcessionCategoryDTO>>(categoryResponse.Result.ToString());
                return categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
            }
            TempData["error"] = categoryResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load category list.";
            return new List<SelectListItem>();
        }
    }
}