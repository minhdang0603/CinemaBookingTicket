using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize(Roles = Constant.Role_Admin)]
    public class ScreenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
