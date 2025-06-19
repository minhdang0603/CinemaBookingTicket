using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Public.Controllers
{
    [Area("Public")]
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}
