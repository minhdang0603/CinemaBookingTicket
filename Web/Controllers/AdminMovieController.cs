using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Web.Models;

namespace Web.Controllers
{
    //  [Authorize(Roles = "Admin")]
    public class AdminMovieController : Controller
    {
        public IActionResult Index()
        {
          
            return View();
        }
        
 }

    
}
