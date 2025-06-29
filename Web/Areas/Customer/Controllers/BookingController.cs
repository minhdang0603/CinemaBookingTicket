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
			// Initialize ViewBag for seat selection page
			ViewBag.SelectedSeats = "...";
			ViewBag.TotalAmount = "0";
			return View();
		}

		public IActionResult Concession()
		{
			// Get data from TempData or session
			ViewBag.SelectedSeats = TempData["SelectedSeats"] ?? "...";
			ViewBag.TotalAmount = TempData["TotalAmount"] ?? "0";
			
			// Keep data for next step
			TempData.Keep("SelectedSeats");
			TempData.Keep("TotalAmount");
			
			return View();
		}

		public IActionResult Payment()
		{
			// Get data from TempData or session
			ViewBag.SelectedSeats = TempData["SelectedSeats"] ?? "...";
			ViewBag.TotalAmount = TempData["TotalAmount"] ?? "0";
			ViewBag.SeatCount = TempData["SeatCount"] ?? "0";
			ViewBag.SeatTotal = TempData["SeatTotal"] ?? "0";
			
			// Keep data for next step
			TempData.Keep("SelectedSeats");
			TempData.Keep("TotalAmount");
			TempData.Keep("SeatCount");
			TempData.Keep("SeatTotal");
			
			return View();
		}

		[HttpPost]
		public IActionResult ProcessPayment(string customerName, string customerPhone, string customerEmail, 
			string paymentMethod, string selectedSeats, string totalAmount, string comboData)
		{
			// Store customer info and booking data in TempData
			TempData["CustomerName"] = customerName;
			TempData["CustomerPhone"] = customerPhone;
			TempData["CustomerEmail"] = customerEmail;
			TempData["PaymentMethod"] = GetPaymentMethodName(paymentMethod);
			TempData["SelectedSeats"] = selectedSeats;
			TempData["TotalAmount"] = totalAmount;
			TempData["ComboData"] = comboData;

			// Redirect to confirmation
			return RedirectToAction("Confirmation");
		}

		public IActionResult Confirmation()
		{
			// Get all booking data
			ViewBag.CustomerName = TempData["CustomerName"] ?? "";
			ViewBag.CustomerPhone = TempData["CustomerPhone"] ?? "";
			ViewBag.CustomerEmail = TempData["CustomerEmail"] ?? "";
			ViewBag.PaymentMethod = TempData["PaymentMethod"] ?? "Chuyển khoản / Quét mã QR";
			ViewBag.SelectedSeats = TempData["SelectedSeats"] ?? "...";
			ViewBag.TotalAmount = TempData["TotalAmount"] ?? "0";
			
			return View();
		}

		// Helper method to convert payment method codes to display names
		private string GetPaymentMethodName(string method)
		{
			return method switch
			{
				"qr" => "Chuyển khoản / Quét mã QR",
				"momo" => "Ví MoMo",
				"card" => "Thẻ ATM / Thẻ quốc tế",
				_ => "Chuyển khoản / Quét mã QR"
			};
		}

		// API endpoints for handling seat selection data
		[HttpPost]
		public IActionResult SaveSeatSelection([FromBody] SeatSelectionData data)
		{
			TempData["SelectedSeats"] = data.SelectedSeats;
			TempData["TotalAmount"] = data.TotalAmount;
			TempData["SeatCount"] = data.SeatCount;
			TempData["SeatTotal"] = data.SeatTotal;
			
			return Json(new { success = true });
		}
	}

	// Model for seat selection data
	public class SeatSelectionData
	{
		public string SelectedSeats { get; set; } = string.Empty;
		public string TotalAmount { get; set; } = string.Empty;
		public string SeatCount { get; set; } = string.Empty;
		public string SeatTotal { get; set; } = string.Empty;
	}
}
