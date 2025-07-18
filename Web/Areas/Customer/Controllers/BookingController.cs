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
			string token = GetUserToken();

			// Check for existing booking and handle it appropriately
			var bookingInfo = await GetExistingBookingInfoAsync(showTimeId, token);
			BookingDTO? existingBooking = bookingInfo.ExistingBooking;

			// Fetch seat status for the showtime
			var showTimeWithSeatStatus = await GetShowTimeSeatStatusAsync(showTimeId, token);
			if (showTimeWithSeatStatus == null)
			{
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			// Create view model for seat booking
			var viewModel = CreateSeatBookingViewModel(showTimeId, showTimeWithSeatStatus);

			// If there's an existing booking, mark selected seats
			if (existingBooking != null && existingBooking.BookingItems != null && existingBooking.BookingItems.Any())
			{
				PopulateExistingBookingData(viewModel, existingBooking, bookingInfo.ExpiryTime);
			}

			return View(viewModel);
		}

		public async Task<IActionResult> Concession()
		{
			// Validate booking session
			var bookingSessionResult = await ValidateBookingSessionAsync();
			if (!bookingSessionResult.IsValid || !bookingSessionResult.BookingId.HasValue || !bookingSessionResult.ExpiryTime.HasValue)
			{
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			int bookingId = bookingSessionResult.BookingId.Value;
			DateTime expiry = bookingSessionResult.ExpiryTime.Value;
			string token = GetUserToken();

			// Get booking details
			var booking = await GetBookingDetailsAsync(bookingId, token);
			if (booking == null)
			{
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			// Get concessions
			var concessions = await GetConcessionsAsync();
			if (concessions == null)
			{
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			// Create view model
			var viewModel = new ConcessionViewModel
			{
				ShowTimeId = booking.ShowTime.Id,
				BookingId = bookingId,
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
			// Validate booking session
			var bookingSessionResult = await ValidateBookingSessionAsync();
			if (!bookingSessionResult.IsValid || !bookingSessionResult.BookingId.HasValue || !bookingSessionResult.ExpiryTime.HasValue)
			{
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			int bookingId = bookingSessionResult.BookingId.Value;
			DateTime expiry = bookingSessionResult.ExpiryTime.Value;
			string token = GetUserToken();

			// Get booking details
			var booking = await GetBookingDetailsAsync(bookingId, token);
			if (booking == null)
			{
				return RedirectToAction("Index", "Home", new { area = "Public" });
			}

			// Create payment view model
			var viewModel = CreatePaymentViewModel(booking, expiry);

			// Add concession order details if they exist
			await AddConcessionOrderDetailsToPaymentModelAsync(viewModel, bookingId, token);

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> ProcessPayment(PaymentViewModel viewModel)
		{
			try
			{
				// Store booking information in TempData for confirmation page
				SaveBookingInfoToTempData(viewModel);

				// Get user token
				string token = GetUserToken();

				// Create VNPay payment request
				var paymentResponse = await CreateVNPayPaymentAsync(viewModel, token);
				if (!paymentResponse.Success)
				{
					TempData["error"] = paymentResponse.ErrorMessage;
					return RedirectToAction("Payment");
				}

				// Redirect to VNPay payment gateway
				return Redirect(paymentResponse.PaymentUrl);
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
				var queryString = Request.QueryString.Value;

				// Process VNPay response if available
				if (!string.IsNullOrEmpty(queryString))
				{
					var vnPayResponse = await ProcessVNPayCheckResponseAsync(queryString);
					if (vnPayResponse != null)
					{
						return View();
					}
				}

				// Fall back to manual confirmation with data from TempData
				ProcessManualConfirmation();
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

		// Action to create booking
		[HttpPost]
		public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDTO bookingCreateDTO)
		{
			try
			{
				// Validate input and user token
				string token = GetUserToken();
				if (string.IsNullOrEmpty(token))
				{
					return Json(new { success = false, message = "User not authenticated" });
				}

				if (!IsValidBookingData(bookingCreateDTO))
				{
					return Json(new { success = false, message = "Invalid booking data" });
				}

				// Create the booking
				var bookingResult = await CreateBookingInDatabaseAsync(bookingCreateDTO, token);
				if (bookingResult == null)
				{
					return Json(new { success = false, message = "Failed to create booking" });
				}

				// Save booking info to session
				SaveBookingToSession(bookingResult.Id);

				return Json(new
				{
					success = true,
					message = "Booking created successfully",
					bookingId = bookingResult.Id,
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

		// Action for updating an existing booking
		[HttpPost]
		public async Task<IActionResult> UpdateBooking([FromBody] BookingUpdateDTO bookingUpdateDTO)
		{
			try
			{
				// Validate input and user token
				string token = GetUserToken();
				if (string.IsNullOrEmpty(token))
				{
					return Json(new { success = false, message = "User not authenticated" });
				}

				if (!IsValidBookingUpdateData(bookingUpdateDTO))
				{
					return Json(new { success = false, message = "Invalid booking update data" });
				}

				// Update the booking
				var bookingResponse = await _bookingService.UpdateBookingAsync<APIResponse>(bookingUpdateDTO, token);

				if (bookingResponse == null || !bookingResponse.IsSuccess)
				{
					return Json(new
					{
						success = false,
						message = bookingResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to update booking"
					});
				}

				// Get updated booking details
				var bookingResult = JsonConvert.DeserializeObject<BookingDTO>(bookingResponse.Result?.ToString() ?? "{}");

				if (bookingResult == null)
				{
					return Json(new { success = false, message = "Failed to update booking" });
				}

				// Update session expiry time
				HttpContext.Session.SetString("BookingExpiry", DateTime.Now.AddMinutes(BOOKING_EXPIRY_MINUTES).ToString("o"));

				return Json(new
				{
					success = true,
					message = "Booking updated successfully",
					bookingId = bookingResult.Id,
					redirect = Url.Action("Concession", "Booking", new { area = "Customer" }),
					expiryMinutes = BOOKING_EXPIRY_MINUTES
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating booking");
				return Json(new { success = false, message = "An unexpected error occurred" });
			}
		}

		// Action to create concession order
		[HttpPost]
		public async Task<IActionResult> CreateConcessionOrder([FromBody] ConcessionOrderCreateDTO concessionOrderCreateDTO)
		{
			try
			{
				// Get authenticated user token
				string token = GetUserToken();
				if (string.IsNullOrEmpty(token))
				{
					return Json(new { success = false, message = "User not authenticated" });
				}

				if (!IsValidConcessionOrderData(concessionOrderCreateDTO))
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

				var concessionOrderResult = JsonConvert.DeserializeObject<ConcessionOrderDTO>(concessionOrderResponse.Result?.ToString() ?? "{}");

				if (concessionOrderResult == null)
				{
					return Json(new { success = false, message = "Failed to create concession order" });
				}

				// Save the concession order ID in session
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

		// Action to cancel booking
		[HttpPost]
		public async Task<IActionResult> CancelBooking()
		{
			int? bookingId = HttpContext.Session.GetInt32("BookingId");
			if (!bookingId.HasValue)
			{
				return Json(new { success = true, message = "No active booking to cancel" });
			}

			try
			{
				string token = GetUserToken();
				await _bookingService.CancelBookingAsync<APIResponse>(bookingId.Value, token);

				ClearBookingSession();

				return Json(new { success = true, message = "Booking canceled successfully" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error canceling booking {BookingId}", bookingId.Value);
				return Json(new { success = false, message = "Failed to cancel booking" });
			}
		}

		#region Helper Methods
		private string GetUserToken()
		{
			return HttpContext.Session.GetString(Constant.SessionToken) ?? "";
		}

		private async Task<(BookingDTO? ExistingBooking, DateTime? ExpiryTime)> GetExistingBookingInfoAsync(int showTimeId, string token)
		{
			int? existingBookingId = HttpContext.Session.GetInt32("BookingId");
			string? expiryString = HttpContext.Session.GetString("BookingExpiry");
			DateTime? expiryTime = null;

			if (!string.IsNullOrEmpty(expiryString) && DateTime.TryParse(expiryString, out DateTime expiry))
			{
				expiryTime = expiry;
			}

			BookingDTO? existingBooking = null;

			if (existingBookingId.HasValue && expiryTime.HasValue && DateTime.Now < expiryTime.Value)
			{
				var existingBookingResponse = await _bookingService.GetBookingByIdAsync<APIResponse>(existingBookingId.Value, token);

				if (existingBookingResponse != null && existingBookingResponse.IsSuccess)
				{
					existingBooking = JsonConvert.DeserializeObject<BookingDTO>(existingBookingResponse.Result?.ToString() ?? "{}");

					if (existingBooking != null && existingBooking.ShowTime.Id != showTimeId)
					{
						_logger.LogInformation("User navigated to different showtime. Clearing previous booking {BookingId}", existingBookingId.Value);
						await _bookingService.DeleteBookingAsync<APIResponse>(existingBookingId.Value, token);
						ClearBookingSession();
						existingBooking = null;
					}
				}
				else
				{
					ClearBookingSession();
				}
			}
			else if (existingBookingId.HasValue)
			{
				await _bookingService.DeleteBookingAsync<APIResponse>(existingBookingId.Value, token);
				ClearBookingSession();
			}

			return (existingBooking, expiryTime);
		}

		private async Task<ShowTimeSeatStatusDTO?> GetShowTimeSeatStatusAsync(int showTimeId, string token)
		{
			var seatStatusResponse = await _showtimeService.GetShowTimeSeatStatusAsync<APIResponse>(showTimeId, token);

			if (seatStatusResponse == null || !seatStatusResponse.IsSuccess)
			{
				_logger.LogError("Failed to retrieve seat status.");
				TempData["error"] = seatStatusResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to retrieve seat status.";
				return null;
			}

			var showTimeWithSeatStatus = JsonConvert.DeserializeObject<ShowTimeSeatStatusDTO>(seatStatusResponse.Result?.ToString() ?? "{}");

			if (showTimeWithSeatStatus == null)
			{
				_logger.LogError("Seat status data is invalid.");
				TempData["error"] = "Seat status data is invalid.";
				return null;
			}

			return showTimeWithSeatStatus;
		}

		private SeatBookingViewModel CreateSeatBookingViewModel(int showTimeId, ShowTimeSeatStatusDTO showTimeData)
		{
			return new SeatBookingViewModel
			{
				ShowtimeId = showTimeId,
				MovieId = showTimeData.MovieId,
				ScreenName = showTimeData.ScreenName,
				MovieTitle = showTimeData.MovieTitle,
				TheaterName = showTimeData.TheaterName,
				ShowtimeDate = showTimeData.ShowDate,
				ShowtimeTime = showTimeData.StartTime,
				Seats = showTimeData.Seats,
				SeatTypes = showTimeData.Seats
					.Select(seat => seat.SeatType)
					.GroupBy(type => type.Id)
					.Select(group => new SeatTypeViewModel
					{
						Id = group.Key,
						Name = group.First()?.Name ?? string.Empty,
						Price = (group.First()?.PriceMultiplier ?? 1) * showTimeData.BasePrice,
						Color = group.First()?.Color ?? "#333333",
					})
					.ToList()
			};
		}

		private void PopulateExistingBookingData(SeatBookingViewModel viewModel, BookingDTO existingBooking, DateTime? expiryTime)
		{
			var existingBookedSeatNames = existingBooking.BookingItems.Select(item => item.SeatName).ToList();

			var existingBookedSeatIds = viewModel.Seats
				.Where(s => existingBookedSeatNames.Contains(s.SeatCode))
				.Select(s => s.Id)
				.ToList();

			ViewBag.ExistingBookingId = existingBooking.Id;
			ViewBag.ExistingBookingExpiry = expiryTime;
			ViewBag.ExistingBookedSeats = string.Join(",", existingBookedSeatIds);
			ViewBag.ExistingBookedSeatNames = string.Join(", ", existingBookedSeatNames);
			ViewBag.ExistingBookingAmount = existingBooking.TotalAmount;
		}

		private async Task<(bool IsValid, int? BookingId, DateTime? ExpiryTime)> ValidateBookingSessionAsync()
		{
			int? bookingId = HttpContext.Session.GetInt32("BookingId");
			string? expiryString = HttpContext.Session.GetString("BookingExpiry");
			string token = GetUserToken();

			if (!bookingId.HasValue || string.IsNullOrEmpty(expiryString) ||
				!DateTime.TryParse(expiryString, out DateTime expiry) || DateTime.Now > expiry)
			{
				if (bookingId.HasValue)
				{
					await _bookingService.DeleteBookingAsync<APIResponse>(bookingId.Value, token);
					_logger.LogInformation("Expired booking {BookingId} was deleted during page access", bookingId.Value);
					ClearBookingSession();
				}

				TempData["error"] = "Thời gian đặt vé của bạn đã hết. Vui lòng chọn ghế lại.";
				return (false, null, null);
			}

			return (true, bookingId, expiry);
		}

		private async Task<BookingDTO?> GetBookingDetailsAsync(int bookingId, string token)
		{
			var bookingResponse = await _bookingService.GetBookingByIdAsync<APIResponse>(bookingId, token);

			if (bookingResponse == null || !bookingResponse.IsSuccess)
			{
				_logger.LogError("Failed to retrieve booking details.");
				TempData["error"] = bookingResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to retrieve booking details.";
				return null;
			}

			var booking = JsonConvert.DeserializeObject<BookingDTO>(bookingResponse.Result?.ToString() ?? "{}");

			if (booking == null)
			{
				_logger.LogError("Booking data is invalid.");
				TempData["error"] = "Booking data is invalid.";
				return null;
			}

			return booking;
		}

		private async Task<List<ConcessionDTO>?> GetConcessionsAsync()
		{
			var concessionResponse = await _concessionService.GetAllConcessionsAsync<APIResponse>();

			if (concessionResponse == null || !concessionResponse.IsSuccess)
			{
				_logger.LogError("Failed to retrieve concession list.");
				TempData["error"] = concessionResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to retrieve concession list.";
				return null;
			}

			var concessions = JsonConvert.DeserializeObject<List<ConcessionDTO>>(concessionResponse.Result?.ToString() ?? "[]");

			if (concessions == null)
			{
				_logger.LogError("Concession data is invalid.");
				TempData["error"] = "Concession data is invalid.";
				return null;
			}

			return concessions;
		}

		private PaymentViewModel CreatePaymentViewModel(BookingDTO booking, DateTime expiryTime)
		{
			return new PaymentViewModel
			{
				BookingId = booking.Id,
				MovieTitle = booking.ShowTime.MovieTitle,
				TheaterName = booking.ShowTime.TheaterName,
				ShowTime = $"{booking.ShowTime.StartTime:HH:mm} {booking.ShowTime.ShowDate:dd/MM/yyyy}",
				ScreenName = booking.ShowTime.ScreenName,
				SeatNames = booking.BookingItems.Select(bd => bd.SeatName).ToList(),
				SeatCount = booking.BookingItems.Count,
				SeatTotal = booking.TotalAmount,
				BookingExpiry = expiryTime,
				TotalAmount = booking.TotalAmount
			};
		}

		private async Task AddConcessionOrderDetailsToPaymentModelAsync(PaymentViewModel viewModel, int bookingId, string token)
		{
			int? concessionOrderId = HttpContext.Session.GetInt32("ConcessionOrderId");
			if (!concessionOrderId.HasValue)
			{
				return;
			}

			var concessionOrdersResponseObj = await _concessionOrderService.GetConcessionOrdersByBookingIdAsync<APIResponse>(bookingId, token);

			if (concessionOrdersResponseObj == null || !concessionOrdersResponseObj.IsSuccess)
			{
				return;
			}

			var concessionOrdersList = JsonConvert.DeserializeObject<List<ConcessionOrderDTO>>(
				concessionOrdersResponseObj.Result?.ToString() ?? "[]");

			if (concessionOrdersList == null || !concessionOrdersList.Any())
			{
				return;
			}

			var concessionOrder = concessionOrdersList.FirstOrDefault(co => co.Id == concessionOrderId.Value);
			if (concessionOrder == null)
			{
				return;
			}

			viewModel.ConcessionOrderId = concessionOrder.Id;
			viewModel.ConcessionItems = concessionOrder.ConcessionOrderItems;
			viewModel.ConcessionCount = concessionOrder.ConcessionOrderItems.Sum(item => item.Quantity);
			viewModel.ConcessionTotal = concessionOrder.TotalAmount;
			viewModel.TotalAmount += concessionOrder.TotalAmount;
		}

		private void SaveBookingInfoToTempData(PaymentViewModel viewModel)
		{
			TempData["BookingId"] = viewModel.BookingId.ToString();
			TempData["ConcessionOrderId"] = viewModel.ConcessionOrderId?.ToString() ?? "";
			TempData["TotalAmount"] = viewModel.TotalAmount.ToString();
			TempData["SelectedSeats"] = string.Join(", ", viewModel.SeatNames);
			TempData["PaymentMethod"] = "VNPAY";
		}

		private async Task<(bool Success, string ErrorMessage, string PaymentUrl)> CreateVNPayPaymentAsync(PaymentViewModel viewModel, string token)
		{
			var paymentResponse = await _paymentService.CreateVNPayPaymentAsync<APIResponse>(new VNPayRequestDTO
			{
				BookingId = viewModel.BookingId,
				Amount = viewModel.TotalAmount,
				OrderInfo = $"Payment for booking {viewModel.BookingId}" +
							(viewModel.ConcessionOrderId.HasValue ? $" with concession order {viewModel.ConcessionOrderId}" : ""),
				ClientIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
			}, token);

			if (paymentResponse == null || !paymentResponse.IsSuccess)
			{
				return (false,
					paymentResponse?.ErrorMessages?.FirstOrDefault() ?? "Failed to create payment.",
					string.Empty);
			}

			var paymentUrl = paymentResponse.Result?.ToString() ?? string.Empty;

			if (string.IsNullOrEmpty(paymentUrl))
			{
				return (false, "Failed to generate payment URL.", string.Empty);
			}

			return (true, string.Empty, paymentUrl);
		}

		private async Task<VNPayResponseDTO?> ProcessVNPayCheckResponseAsync(string queryString)
		{
			var vnPayCheckResponse = await _paymentService.VNPayCheckAsync<APIResponse>(queryString);

			if (vnPayCheckResponse == null || !vnPayCheckResponse.IsSuccess)
			{
				return null;
			}

			var vnPayResponse = JsonConvert.DeserializeObject<VNPayResponseDTO>(vnPayCheckResponse.Result?.ToString() ?? "{}");

			if (vnPayResponse == null)
			{
				return null;
			}

			ViewBag.PaymentStatus = vnPayResponse.Success ? "success" : "failed";
			ViewBag.PaymentMessage = vnPayResponse.Message;

			ViewBag.BookingId = vnPayResponse.OrderId;
			ViewBag.BookingCode = vnPayResponse.BookingCode;
			ViewBag.TransactionId = vnPayResponse.TransactionId;
			ViewBag.TotalAmount = vnPayResponse.Amount.ToString("N0");
			ViewBag.PaymentMethod = "VNPAY";

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

			if (vnPayResponse.Success)
			{
				ClearBookingSession();
			}

			return vnPayResponse;
		}

		private void ProcessManualConfirmation()
		{
			string vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();
			bool isSuccess = vnp_ResponseCode == "00";

			ViewBag.PaymentStatus = isSuccess ? "success" : "failed";
			ViewBag.PaymentMessage = isSuccess ? "Thanh toán thành công!" : "Thanh toán thất bại hoặc bị hủy.";

			ViewBag.BookingId = TempData["BookingId"] ?? "0";
			ViewBag.ConcessionOrderId = TempData["ConcessionOrderId"] ?? "";
			ViewBag.SelectedSeats = TempData["SelectedSeats"] ?? "...";
			ViewBag.TotalAmount = TempData["TotalAmount"] ?? "0";
			ViewBag.PaymentMethod = TempData["PaymentMethod"] ?? "VNPAY";

			if (isSuccess)
			{
				ClearBookingSession();
			}
		}

		private void ClearBookingSession()
		{
			HttpContext.Session.Remove("BookingId");
			HttpContext.Session.Remove("BookingExpiry");
			HttpContext.Session.Remove("ConcessionOrderId");
		}

		private bool IsValidBookingData(BookingCreateDTO bookingCreateDTO)
		{
			return bookingCreateDTO.ShowTimeId > 0 &&
				   bookingCreateDTO.BookingDetails != null &&
				   bookingCreateDTO.BookingDetails.Any();
		}

		private bool IsValidBookingUpdateData(BookingUpdateDTO bookingUpdateDTO)
		{
			return bookingUpdateDTO.BookingId > 0 &&
				   bookingUpdateDTO.ShowTimeId > 0 &&
				   bookingUpdateDTO.BookingDetails != null &&
				   bookingUpdateDTO.BookingDetails.Any();
		}

		private bool IsValidConcessionOrderData(ConcessionOrderCreateDTO concessionOrderCreateDTO)
		{
			return concessionOrderCreateDTO.BookingId > 0 &&
				   concessionOrderCreateDTO.ConcessionOrderDetails != null &&
				   concessionOrderCreateDTO.ConcessionOrderDetails.Any();
		}

		private void SaveBookingToSession(int bookingId)
		{
			HttpContext.Session.SetInt32("BookingId", bookingId);
			HttpContext.Session.SetString("BookingExpiry", DateTime.Now.AddMinutes(BOOKING_EXPIRY_MINUTES).ToString("o"));
		}

		private async Task<BookingDTO?> CreateBookingInDatabaseAsync(BookingCreateDTO bookingCreateDTO, string token)
		{
			var bookingCreateResponse = await _bookingService.CreateBookingAsync<APIResponse>(bookingCreateDTO, token);

			if (bookingCreateResponse == null || !bookingCreateResponse.IsSuccess)
			{
				_logger.LogError("Failed to create booking: {Error}",
					bookingCreateResponse?.ErrorMessages?.FirstOrDefault() ?? "Unknown error");
				return null;
			}

			return JsonConvert.DeserializeObject<BookingDTO>(bookingCreateResponse.Result?.ToString() ?? "{}");
		}
		#endregion
	}
}
