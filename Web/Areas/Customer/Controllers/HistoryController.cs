using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class HistoryController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
