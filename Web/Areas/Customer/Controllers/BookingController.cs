using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Models.ViewModels;
using Web.Services.IServices;

namespace Web.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class BookingController : Controller
	{
		private readonly IShowtimeService _showtimeService;
		private readonly IBookingService _bookingService;
		private readonly IPaymentService _paymentService;
		private readonly IConcessionService _concessionService;
		private readonly IScreenService _screenService;
		private readonly ILogger<BookingController> _logger;

		// Thêm constant cho thời gian hết hạn booking (5 phút)
		private const int BOOKING_EXPIRY_MINUTES = 5;

		public BookingController(IShowtimeService showtimeService, IBookingService bookingService, IPaymentService paymentService, IConcessionService concessionService, IScreenService screenService, ILogger<BookingController> logger)
		{
			_showtimeService = showtimeService;
			_bookingService = bookingService;
			_paymentService = paymentService;
			_concessionService = concessionService;
			_screenService = screenService;
			_logger = logger;
		}


		public async Task<IActionResult> Index(int showTimeId)
		{
			// Get authenticated user token
			string token = HttpContext.Session.GetString(Constant.SessionToken) ?? "";

			// Fetch seat status for the showtime
			var seatStatusResponse = await _showtimeService.GetShowTimeSeatStatusAsync<APIResponse>(showTimeId, token);

			if (seatStatusResponse == null || !seatStatusResponse.IsSuccess)
			{
				_logger.LogError("Failed to retrieve seat status.");
				TempData["error"] = seatStatusResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to retrieve seat status.";
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			var showTimeWithSeatStatus = JsonConvert.DeserializeObject<ShowTimeSeatStatusDTO>(seatStatusResponse.Result?.ToString() ?? "{}");

			if (showTimeWithSeatStatus == null)
			{
				_logger.LogError("Seat status data is invalid.");
				TempData["error"] = "Seat status data is invalid.";
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			// Create view model for seat booking
			var viewModel = new SeatBookingViewModel
			{
				ShowtimeId = showTimeId,
				MovieId = showTimeWithSeatStatus.MovieId,
				ScreenName = showTimeWithSeatStatus.ScreenName,
				MovieTitle = showTimeWithSeatStatus.MovieTitle,
				TheaterName = showTimeWithSeatStatus.TheaterName,
				ShowtimeDate = showTimeWithSeatStatus.ShowDate,
				ShowtimeTime = showTimeWithSeatStatus.StartTime,
				Seats = showTimeWithSeatStatus.Seats,
				SeatTypes = showTimeWithSeatStatus.Seats
					.Select(seat => seat.SeatType)
					.GroupBy(type => type.Id)
					.Select(group => new SeatTypeViewModel
					{
						Id = group.Key,
						Name = group.First().Name,
						Price = group.First().PriceMultiplier * showTimeWithSeatStatus.BasePrice,
						Color = group.First().Color,
					})
					.ToList()
			};


			return View(viewModel);
		}

		public async Task<IActionResult> Concession()
		{
			// Kiểm tra booking có còn hiệu lực không
			int? bookingId = HttpContext.Session.GetInt32("BookingId");
			string? expiryString = HttpContext.Session.GetString("BookingExpiry");
			string token = HttpContext.Session.GetString(Constant.SessionToken) ?? "";

			if (!bookingId.HasValue || string.IsNullOrEmpty(expiryString) ||
				!DateTime.TryParse(expiryString, out DateTime expiry) || DateTime.Now > expiry)
			{
				// Nếu booking đã hết hạn, xóa hẳn booking trong database và clear session
				if (bookingId.HasValue)
				{
					await _bookingService.DeleteBookingAsync<APIResponse>(bookingId.Value, token);
					_logger.LogInformation("Expired booking {BookingId} was deleted during Concession page access", bookingId.Value);
					ClearBookingSession();
				}

				TempData["error"] = "Thời gian đặt vé của bạn đã hết. Vui lòng chọn ghế lại.";
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			// Lấy thông tin booking từ API
			var bookingResponse = await _bookingService.GetBookingByIdAsync<APIResponse>(bookingId.Value, token);

			if (bookingResponse == null || !bookingResponse.IsSuccess)
			{
				_logger.LogError("Failed to retrieve booking details.");
				TempData["error"] = bookingResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to retrieve booking details.";
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			var booking = JsonConvert.DeserializeObject<BookingDTO>(bookingResponse.Result?.ToString() ?? "{}");

			if (booking == null)
			{
				_logger.LogError("Booking data is invalid.");
				TempData["error"] = "Booking data is invalid.";
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			// Lấy danh sách concession từ API
			var concessionResponse = await _concessionService.GetAllConcessionsAsync<APIResponse>();

			if (concessionResponse == null || !concessionResponse.IsSuccess)
			{
				_logger.LogError("Failed to retrieve concession list.");
				TempData["error"] = concessionResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to retrieve concession list.";
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			var concessions = JsonConvert.DeserializeObject<List<ConcessionDTO>>(concessionResponse.Result?.ToString() ?? "[]");

			if (concessions == null)
			{
				_logger.LogError("Concession data is invalid.");
				TempData["error"] = "Concession data is invalid.";
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			var viewModel = new ConcessionViewModel
			{
				ShowTimeId = booking.ShowTime.Id,
				MovieTitle = booking.ShowTime.MovieTitle,
				CinemaName = booking.ShowTime.TheaterName,
				ShowTime = $"{booking.ShowTime.StartTime.ToString("HH:mm")} {booking.ShowTime.ShowDate.ToString("dd/MM/yyyy")}",
				ScreenName = booking.ShowTime.ScreenName,
				SeatName = booking.BookingItems.Select(bd => bd.SeatName).ToList(),
				TotalAmount = (int)booking.TotalAmount,
				Concessions = concessions
			};

			ViewBag.BookingExpiry = expiry;

			return View(viewModel);
		}

		public async Task<IActionResult> Payment()
		{
			// Kiểm tra booking có còn hiệu lực không
			int? bookingId = HttpContext.Session.GetInt32("BookingId");
			string? expiryString = HttpContext.Session.GetString("BookingExpiry");

			if (!bookingId.HasValue || string.IsNullOrEmpty(expiryString) ||
				!DateTime.TryParse(expiryString, out DateTime expiry) || DateTime.Now > expiry)
			{
				// Nếu booking đã hết hạn, xóa hẳn booking trong database và clear session
				if (bookingId.HasValue)
				{
					string token = HttpContext.Session.GetString(Constant.SessionToken) ?? "";
					await _bookingService.DeleteBookingAsync<APIResponse>(bookingId.Value, token);
					_logger.LogInformation("Expired booking {BookingId} was deleted during Payment page access", bookingId.Value);
					ClearBookingSession();
				}

				TempData["error"] = "Thời gian đặt vé của bạn đã hết. Vui lòng chọn ghế lại.";
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			// Booking còn hiệu lực, tiếp tục hiển thị trang
			ViewBag.SelectedSeats = TempData["SelectedSeats"] ?? "...";
			ViewBag.TotalAmount = TempData["TotalAmount"] ?? "0";
			ViewBag.SeatCount = TempData["SeatCount"] ?? "0";
			ViewBag.SeatTotal = TempData["SeatTotal"] ?? "0";
			ViewBag.BookingExpiry = expiry;
			ViewBag.RemainingMinutes = Math.Max(0, (int)(expiry - DateTime.Now).TotalMinutes);

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

		// Thêm action để tạo booking
		[HttpPost]
		public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDTO bookingCreateDTO)
		{
			try
			{
				// Get authenticated user token
				string token = HttpContext.Session.GetString(Constant.SessionToken) ?? "";

				if (string.IsNullOrEmpty(token))
				{
					return Json(new { success = false, message = "User not authenticated" });
				}

				if (bookingCreateDTO.ShowTimeId <= 0 || bookingCreateDTO.BookingDetails == null || !bookingCreateDTO.BookingDetails.Any())
				{
					return Json(new { success = false, message = "Invalid booking data" });
				}

				// Lấy thông tin showtime để kiểm tra
				var showtimeResponse = await _showtimeService.GetShowTimeSeatStatusAsync<APIResponse>(bookingCreateDTO.ShowTimeId, token);

				var bookingCreateResponse = await _bookingService.CreateBookingAsync<APIResponse>(bookingCreateDTO, token);

				if (bookingCreateResponse == null || !bookingCreateResponse.IsSuccess)
				{
					return Json(new
					{
						success = false,
						message = bookingCreateResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to create booking"
					});
				}

				// Chuyển thông tin booking vào dynamic result
				var bookingResult = JsonConvert.DeserializeObject<BookingDTO>(bookingCreateResponse.Result?.ToString() ?? "{}");

				if (bookingResult == null)
				{
					return Json(new { success = false, message = "Failed to create booking" });
				}

				int bookingId = bookingResult.Id;

				// Lưu ID booking và thời hạn vào Session
				HttpContext.Session.SetInt32("BookingId", bookingId);
				HttpContext.Session.SetString("BookingExpiry", DateTime.Now.AddMinutes(BOOKING_EXPIRY_MINUTES).ToString("o"));

				return Json(new
				{
					success = true,
					message = "Booking created successfully",
					bookingId = bookingId,
					redirect = Url.Action("Concession", "Booking", new { area = "Customer" }),
					expiryMinutes = BOOKING_EXPIRY_MINUTES
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating booking");
				return Json(new { success = false, message = "An unexpected error occurred" });
			}
		}

		// Helper method để xóa thông tin booking khỏi session
		private void ClearBookingSession()
		{
			HttpContext.Session.Remove("BookingId");
			HttpContext.Session.Remove("BookingExpiry");
		}

		// Action để xóa booking (khi người dùng hủy thủ công hoặc hết thời gian)
		[HttpPost]
		public async Task<IActionResult> CancelBooking()
		{
			int? bookingId = HttpContext.Session.GetInt32("BookingId");
			if (!bookingId.HasValue)
			{
				return Json(new { success = false, message = "No active booking found" });
			}

			// Gọi API DeleteBookingAsync để xóa hẳn booking
			string token = HttpContext.Session.GetString(Constant.SessionToken) ?? "";
			var response = await _bookingService.DeleteBookingAsync<APIResponse>(bookingId.Value, token);

			_logger.LogInformation("Booking {BookingId} was manually cancelled", bookingId.Value);

			// Xóa thông tin booking từ session
			ClearBookingSession();

			return Json(new
			{
				success = response != null && response.IsSuccess,
				message = response != null && response.IsSuccess ?
					"Booking canceled successfully" :
					"Failed to cancel booking, but session cleared"
			});
		}
	}
}
