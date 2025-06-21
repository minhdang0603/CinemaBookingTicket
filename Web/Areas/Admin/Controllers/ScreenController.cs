using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    // [Authorize(Roles = Constant.Role_Admin)]
    public class ScreenController : Controller
    {
        private readonly IScreenService _screenService;
        private readonly ITheaterService _theaterService;
        private readonly ILogger<ScreenController> _logger;
        private const int PageSize = 10; // Define a constant for page size

        public ScreenController(IScreenService screenService, ITheaterService theaterService, ILogger<ScreenController> logger)
        {
            _screenService = screenService;
            _theaterService = theaterService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Fake data for demo
            var theaters = new List<TheaterDTO>
            {
                new TheaterDTO { Id = 1, Name = "CGV Vincom Center" },
                new TheaterDTO { Id = 2, Name = "Galaxy Cinema" },
                new TheaterDTO { Id = 3, Name = "Lotte Cinema" }
            };
            ViewBag.Theaters = theaters;

            var screens = new List<ScreenDTO>
            {
                new ScreenDTO { Id = 101, Name = "Screen 1", Rows = 10, SeatsPerRow = 20, Theater = theaters[0] },
                new ScreenDTO { Id = 102, Name = "Screen 2", Rows = 8, SeatsPerRow = 15, Theater = theaters[1] },
                new ScreenDTO { Id = 103, Name = "Screen 3", Rows = 12, SeatsPerRow = 18, Theater = theaters[2] },
                new ScreenDTO { Id = 104, Name = "Screen 4", Rows = 9, SeatsPerRow = 16, Theater = theaters[0] },
                new ScreenDTO { Id = 105, Name = "Screen 5", Rows = 11, SeatsPerRow = 22, Theater = theaters[1] },
                new ScreenDTO { Id = 106, Name = "Screen 6", Rows = 7, SeatsPerRow = 14, Theater = theaters[2] }
            };

            ScreenVM screenVM = new ScreenVM
            {
                ScreenList = screens
            };
            return View(screenVM);
        }

        public IActionResult Create()
        {
            // Tạo ScreenCreateDTO mới và trả về PartialView
            var model = new ScreenCreateDTO();

            // Load dữ liệu theaters vào ViewBag để dùng trong dropdown
            LoadTheaterDropdown();

            // Trả về partial view với model
            return PartialView("_CreateScreenModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScreenCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _screenService.CreateScreenAsync<APIResponse>(model);

                if (response != null && response.IsSuccess)
                {
                    // Trả về JSON result để xử lý AJAX
                    return Json(new { success = true, message = "Thêm phòng chiếu mới thành công!" });
                }
                // Nếu có lỗi từ API
                if (response?.ErrorMessages?.Any() == true)
                {
                    foreach (var error in response.ErrorMessages)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo phòng chiếu.");
                }
            }

            // Nếu ModelState không hợp lệ hoặc API trả về lỗi
            LoadTheaterDropdown();
            return PartialView("_CreateScreenModal", model);
        }

        // Helper method to load theaters for dropdown
        private void LoadTheaterDropdown()
        {
            // TODO: Implement theater service and load real theaters
            // For now, using sample data
            ViewBag.Theaters = new List<dynamic>
            {
                new { Id = 1, Name = "CGV Vincom Center" },
                new { Id = 2, Name = "Galaxy Cinema" },
                new { Id = 3, Name = "Lotte Cinema" }
            };
        }
    }
}
