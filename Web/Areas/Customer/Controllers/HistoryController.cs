using API.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;
using Web.Models;
using Web.Models.DTOs.Response;
using Web.Services.IServices;

namespace Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly IBookingService _bookingService;

        public HistoryController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString(Constant.SessionToken);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth", new { area = "Public" });
            }

            var response = await _bookingService.GetMyBookingsAsync<APIResponse>(token);
            if (response == null || !response.IsSuccess)
            {
                return View(new List<BookingHistoryViewModel>());
            }

            var bookings = JsonConvert.DeserializeObject<List<MyBookingDTO>>(
                Convert.ToString(response.Result) ?? "[]");

            if (bookings == null || !bookings.Any())
            {
                return View(new List<BookingHistoryViewModel>());
            }

            var bookingHistoryList = bookings
                .Select(b => new BookingHistoryViewModel
                {
                    Id = b.Id,
                    BookingCode = b.BookingCode,
                    BookingDate = b.BookingDate,
                    Status = b.BookingStatus,
                    MovieTitle = b.ShowTime.MovieTitle,
                    //MoviePosterUrl = b.ShowTime.MoviePosterUrl,
                    TheaterName = b.ShowTime.TheaterName,
                    ScreenName = b.ShowTime.ScreenName,
                    ShowtimeDate = DateTime.Parse(b.ShowTime.ShowDate.ToString()),
                    ShowtimeTime = b.ShowTime.StartTime,
                    SeatNames = string.Join(", ", b.BookingItems.Select(i => i.SeatName)),
                    TotalAmount = b.TotalAmount,
                    TotalTickets = b.BookingItems.Count
                })
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            return View(bookingHistoryList);
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var token = HttpContext.Session.GetString(Constant.SessionToken);
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện thao tác này" });
            }

            try
            {
                await _bookingService.CancelBookingAsync(bookingId, token);
                return Json(new { success = true, message = "Vé đã được hủy thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Không thể hủy vé. Chi tiết: {ex.Message}" });
            }
        }
    }
}
