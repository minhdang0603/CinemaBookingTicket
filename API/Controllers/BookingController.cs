using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using API.DTOs;   

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("history")]
        public async Task<APIResponse<object>> GetBookingHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var bookings = await _bookingService.GetUserBookingHistoryAsync(userId, page, pageSize);
            var totalCount = await _bookingService.GetUserBookingCountAsync(userId);

            var response = new
            {
                Bookings = bookings,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return APIResponse<object>.Builder()
                .WithResult(response)
                .WithSuccess(true)
                .WithStatusCode(HttpStatusCode.OK)
                .Build();
        }

        [HttpGet("{bookingId}")]
        public async Task<APIResponse<BookingDetailDTO>> GetBookingDetail(int bookingId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var booking = await _bookingService.GetBookingDetailAsync(bookingId, userId);

            return APIResponse<BookingDetailDTO>.Builder()
                .WithResult(booking)
                .WithSuccess(true)
                .WithStatusCode(HttpStatusCode.OK)
                .Build();
        }
    }
}
