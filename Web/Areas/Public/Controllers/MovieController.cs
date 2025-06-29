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
<<<<<<< HEAD
        public IActionResult MovieBooking()
        {
            return View();
        }
        public IActionResult MovieList()
        {
            return View();
        }
=======
>>>>>>> 5d3743d5d744ff013ef5b5b75f394a78f70f2f13
    }

}
