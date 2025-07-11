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
		private readonly IConcessionOrderService _concessionOrderService;
		private readonly IScreenService _screenService;
		private readonly ILogger<BookingController> _logger;

		// Thêm constant cho thời gian hết hạn booking (5 phút)
		private const int BOOKING_EXPIRY_MINUTES = 5;

		public BookingController(
			IShowtimeService showtimeService,
			IBookingService bookingService,
			IPaymentService paymentService,
			IConcessionService concessionService,
			IConcessionOrderService concessionOrderService,
			IScreenService screenService,
			ILogger<BookingController> logger)
		{
			_showtimeService = showtimeService;
			_bookingService = bookingService;
			_paymentService = paymentService;
			_concessionService = concessionService;
			_concessionOrderService = concessionOrderService;
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
				BookingId = bookingId.Value,
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
			string token = HttpContext.Session.GetString(Constant.SessionToken) ?? "";

			if (!bookingId.HasValue || string.IsNullOrEmpty(expiryString) ||
				!DateTime.TryParse(expiryString, out DateTime expiry) || DateTime.Now > expiry)
			{
				// Nếu booking đã hết hạn, xóa hẳn booking trong database và clear session
				if (bookingId.HasValue)
				{
					await _bookingService.DeleteBookingAsync<APIResponse>(bookingId.Value, token);
					_logger.LogInformation("Expired booking {BookingId} was deleted during Payment page access", bookingId.Value);
					ClearBookingSession();
				}

				TempData["error"] = "Thời gian đặt vé của bạn đã hết. Vui lòng chọn ghế lại.";
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			// Lấy thông tin booking từ API
			var bookingResponse = await _bookingService.GetBookingByIdAsync<APIResponse>(bookingId.Value, token);

			if (bookingResponse == null || !bookingResponse.IsSuccess)
			{
				_logger.LogError("Failed to retrieve booking details for payment page.");
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

			// Tạo view model cho trang payment
			var viewModel = new PaymentViewModel
			{
				BookingId = booking.Id,
				MovieTitle = booking.ShowTime.MovieTitle,
				TheaterName = booking.ShowTime.TheaterName,
				ShowTime = $"{booking.ShowTime.StartTime.ToString("HH:mm")} {booking.ShowTime.ShowDate.ToString("dd/MM/yyyy")}",
				ScreenName = booking.ShowTime.ScreenName,
				SeatNames = booking.BookingItems.Select(bd => bd.SeatName).ToList(),
				SeatCount = booking.BookingItems.Count,
				SeatTotal = booking.TotalAmount,
				BookingExpiry = expiry,
				TotalAmount = booking.TotalAmount
			};

			// Kiểm tra nếu có concession order
			int? concessionOrderId = HttpContext.Session.GetInt32("ConcessionOrderId");
			if (concessionOrderId.HasValue)
			{
				// Lấy thông tin concession order từ API
				var concessionOrdersResponseObj = await _concessionOrderService.GetConcessionOrdersByBookingIdAsync<APIResponse>(bookingId.Value, token);

				List<ConcessionOrderDTO> concessionOrdersResponse = new List<ConcessionOrderDTO>();
				if (concessionOrdersResponseObj != null && concessionOrdersResponseObj.IsSuccess)
				{
					var deserializedList = JsonConvert.DeserializeObject<List<ConcessionOrderDTO>>(
						concessionOrdersResponseObj.Result?.ToString() ?? "[]");
					if (deserializedList != null)
					{
						concessionOrdersResponse = deserializedList;
					}
				}

				if (concessionOrdersResponse != null && concessionOrdersResponse.Any())
				{
					var concessionOrder = concessionOrdersResponse.FirstOrDefault(co => co.Id == concessionOrderId.Value);

					if (concessionOrder != null)
					{
						viewModel.ConcessionOrderId = concessionOrder.Id;
						viewModel.ConcessionItems = concessionOrder.ConcessionOrderItems;
						viewModel.ConcessionCount = concessionOrder.ConcessionOrderItems.Sum(item => item.Quantity);
						viewModel.ConcessionTotal = concessionOrder.TotalAmount;
						viewModel.TotalAmount += concessionOrder.TotalAmount;
					}
				}
			}

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> ProcessPayment(PaymentViewModel viewModel)
		{
			try
			{
				// Lưu thông tin thanh toán vào TempData để hiển thị ở trang confirmation
				TempData["BookingId"] = viewModel.BookingId.ToString();
				TempData["ConcessionOrderId"] = viewModel.ConcessionOrderId?.ToString() ?? "";
				TempData["TotalAmount"] = viewModel.TotalAmount.ToString();
				TempData["SelectedSeats"] = string.Join(", ", viewModel.SeatNames);
				TempData["PaymentMethod"] = "VNPAY";

				// Lấy token cho API call
				string token = HttpContext.Session.GetString(Constant.SessionToken) ?? "";

				// Tạo yêu cầu thanh toán qua VNPay
				var paymentResponse = await _paymentService.CreateVNPayPaymentAsync<APIResponse>(new VNPayRequestDTO
				{
					BookingId = viewModel.BookingId,
					Amount = viewModel.TotalAmount,
					OrderInfo = $"Payment for booking {viewModel.BookingId}" + (viewModel.ConcessionOrderId.HasValue ? $" with concession order {viewModel.ConcessionOrderId}" : ""),
					ClientIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
				}, token);

				if (paymentResponse == null || !paymentResponse.IsSuccess)
				{
					TempData["error"] = paymentResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to create payment.";
					return RedirectToAction("Payment");
				}

				// Lấy URL để redirect đến cổng thanh toán VNPay
				var paymentUrl = paymentResponse.Result.ToString();

				if (string.IsNullOrEmpty(paymentUrl))
				{
					TempData["error"] = "Failed to generate payment URL.";
					return RedirectToAction("Payment");
				}

				// Redirect đến cổng thanh toán VNPay
				return Redirect(paymentUrl);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing payment");
				TempData["error"] = "An unexpected error occurred while processing payment.";
				return RedirectToAction("Payment");
			}
		}

		public async Task<IActionResult> Confirmation()
		{
			try
			{
				// Tạo queryString từ Request.Query để gọi API vnpay-check
				var queryString = Request.QueryString.Value;

				// Nếu có query string (được gọi từ VNPay redirect), thực hiện kiểm tra thanh toán
				if (!string.IsNullOrEmpty(queryString))
				{
					// Gọi API vnpay-check để xác minh kết quả thanh toán
					var vnPayCheckResponse = await _paymentService.VNPayCheckAsync<APIResponse>(queryString);

					if (vnPayCheckResponse != null && vnPayCheckResponse.IsSuccess)
					{
						// Chuyển đổi kết quả thành VNPayResponseDTO
						var vnPayResponse = JsonConvert.DeserializeObject<VNPayResponseDTO>(vnPayCheckResponse.Result?.ToString() ?? "{}");

						if (vnPayResponse != null)
						{
							// Cập nhật trạng thái thanh toán dựa trên kết quả từ API
							ViewBag.PaymentStatus = vnPayResponse.Success ? "success" : "failed";
							ViewBag.PaymentMessage = vnPayResponse.Message;

							// Hiển thị thông tin đặt vé (sử dụng BookingCode thay vì BookingId)
							ViewBag.BookingId = vnPayResponse.OrderId;
							ViewBag.BookingCode = vnPayResponse.BookingCode;
							ViewBag.TransactionId = vnPayResponse.TransactionId;
							ViewBag.TotalAmount = vnPayResponse.Amount.ToString("N0");
							ViewBag.PaymentMethod = "VNPAY";

							// Hiển thị thông tin chi tiết từ API nếu có
							if (!string.IsNullOrEmpty(vnPayResponse.MovieTitle))
							{
								ViewBag.MovieTitle = vnPayResponse.MovieTitle;
								ViewBag.TheaterName = vnPayResponse.TheaterName;
								ViewBag.ScreenName = vnPayResponse.ScreenName;
								ViewBag.ShowDate = vnPayResponse.ShowDate;
								ViewBag.ShowTime = vnPayResponse.ShowTime;
								ViewBag.SelectedSeats = string.Join(", ", vnPayResponse.SeatNames);
								ViewBag.CustomerName = vnPayResponse.CustomerName;
							}

							// Nếu thanh toán thành công, xóa thông tin booking session vì không cần nữa
							if (vnPayResponse.Success)
							{
								ClearBookingSession();
							}

							return View();
						}
					}
				}

				// Nếu không có query string hoặc API không trả về kết quả, sử dụng phương pháp cũ
				string vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();

				if (vnp_ResponseCode == "00") // Thanh toán thành công
				{
					ViewBag.PaymentStatus = "success";
					ViewBag.PaymentMessage = "Thanh toán thành công!";
				}
				else // Thanh toán thất bại hoặc bị hủy
				{
					ViewBag.PaymentStatus = "failed";
					ViewBag.PaymentMessage = "Thanh toán thất bại hoặc bị hủy.";
				}

				// Hiển thị thông tin đặt vé từ TempData
				ViewBag.BookingId = TempData["BookingId"] ?? "0";
				ViewBag.ConcessionOrderId = TempData["ConcessionOrderId"] ?? "";
				ViewBag.SelectedSeats = TempData["SelectedSeats"] ?? "...";
				ViewBag.TotalAmount = TempData["TotalAmount"] ?? "0";
				ViewBag.PaymentMethod = TempData["PaymentMethod"] ?? "VNPAY";

				// Nếu thanh toán thành công, xóa thông tin booking session
				if (vnp_ResponseCode == "00")
				{
					ClearBookingSession();
				}

				return View();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing payment confirmation");
				ViewBag.PaymentStatus = "failed";
				ViewBag.PaymentMessage = "Đã xảy ra lỗi khi xử lý xác nhận thanh toán.";
				return View();
			}
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
			HttpContext.Session.Remove("ConcessionOrderId");
		}

		// Action để tạo concession order
		[HttpPost]
		public async Task<IActionResult> CreateConcessionOrder([FromBody] Web.Models.DTOs.Request.ConcessionOrderCreateDTO concessionOrderCreateDTO)
		{
			try
			{
				// Get authenticated user token
				string token = HttpContext.Session.GetString(Constant.SessionToken) ?? "";

				if (string.IsNullOrEmpty(token))
				{
					return Json(new { success = false, message = "User not authenticated" });
				}

				if (concessionOrderCreateDTO.BookingId <= 0 || concessionOrderCreateDTO.ConcessionOrderDetails == null || !concessionOrderCreateDTO.ConcessionOrderDetails.Any())
				{
					return Json(new { success = false, message = "Invalid concession order data" });
				}

				var concessionOrderResponse = await _concessionOrderService.CreateConcessionOrderAsync<APIResponse>(concessionOrderCreateDTO, token);

				if (concessionOrderResponse == null || !concessionOrderResponse.IsSuccess)
				{
					return Json(new
					{
						success = false,
						message = concessionOrderResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to create concession order"
					});
				}

				// Deserialize the result to get concession order details
				var concessionOrderResult = JsonConvert.DeserializeObject<ConcessionOrderDTO>(
					concessionOrderResponse.Result?.ToString() ?? "{}");

				if (concessionOrderResult == null)
				{
					return Json(new { success = false, message = "Failed to process concession order data" });
				}

				// Lưu ID concession order vào Session để sử dụng sau này nếu cần
				HttpContext.Session.SetInt32("ConcessionOrderId", concessionOrderResult.Id);

				return Json(new
				{
					success = true,
					message = "Concession order created successfully",
					concessionOrderId = concessionOrderResult.Id,
					redirect = Url.Action("Payment", "Booking", new { area = "Customer" })
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating concession order");
				return Json(new { success = false, message = "An unexpected error occurred" });
			}
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
