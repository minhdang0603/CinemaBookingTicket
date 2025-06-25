using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Utility;
using Web.Models;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize(Roles = Constant.Role_Admin)]
    public class MovieController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }

    }
}
