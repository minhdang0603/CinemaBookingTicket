using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class ProvinceController : Controller
    {
        private readonly IProvinceService _provinceService;
        private readonly ILogger<ProvinceController> _logger;
        private readonly IMapper _mapper;

        public ProvinceController(IProvinceService provinceService, IMapper mapper, ILogger<ProvinceController> logger)
        {
            _provinceService = provinceService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _provinceService.GetAllProvincesAsync<APIResponse>();

            if (response == null || !response.IsSuccess)
            {
                _logger.LogError("Failed to load provinces from API");
                TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to load province list.";
                return View(new List<ProvinceDTO>());
            }

            var provinces = JsonConvert.DeserializeObject<List<ProvinceDTO>>(response.Result.ToString() ?? "[]");
            return View(provinces);
        }

        public IActionResult Create()
        {
            return View(new ProvinceCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProvinceCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _provinceService.CreateProvinceAsync<APIResponse>(model, token);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Province created successfully!";
                return RedirectToAction("Index");
            }

            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to create province.";
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _provinceService.GetProvinceByIdAsync<APIResponse>(id);

            if (response == null || !response.IsSuccess)
            {
                TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to load province.";
                return RedirectToAction("Index");
            }

            var province = JsonConvert.DeserializeObject<ProvinceDetailDTO>(response.Result?.ToString() ?? "{}");
            var model = _mapper.Map<ProvinceUpdateDTO>(province);
            ViewBag.ProvinceId = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProvinceUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProvinceId = id;
                return View(model);
            }

            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _provinceService.UpdateProvinceAsync<APIResponse>(id, model, token);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Province updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to update province.";
            ViewBag.ProvinceId = id;
            return View(model);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString(Constant.SessionToken);
            var response = await _provinceService.DeleteProvinceAsync<APIResponse>(id, token);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Province deleted successfully!";
                return Json(new { success = true });
            }

            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to delete province.";
            return RedirectToAction(nameof(Index));
        }
    }
}
