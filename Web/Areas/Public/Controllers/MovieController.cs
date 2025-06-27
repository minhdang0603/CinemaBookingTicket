using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Public.Controllers
{
    [Area("Public")]
    public class MovieController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MovieBooking()
        {
            return View();
        }
    }

}
