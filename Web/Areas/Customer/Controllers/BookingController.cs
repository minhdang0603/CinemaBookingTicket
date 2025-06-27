using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Customer.Controllers
{
	[Area("Customer")]
	//[Authorize]
	public class BookingController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
