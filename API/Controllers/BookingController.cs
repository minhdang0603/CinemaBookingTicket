using API.Data.Models;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using brevo_csharp.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utility;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<APIResponse<BookingDTO>>> CreateBookingAsync([FromBody] BookingCreateDTO bookingCreateDTO)
    {
        var booking = await _bookingService.CreateBookingAsync(bookingCreateDTO);
        return CreatedAtRoute("GetBookingByIdAsync", new { bookingId = booking.Id }, APIResponse<BookingDTO>.Builder()
            .WithResult(booking)
            .WithStatusCode(HttpStatusCode.Created)
            .WithSuccess(true)
            .Build());
    }

    [HttpDelete("{bookingId:int}")]
    [Authorize]
    public async Task<ActionResult<APIResponse<object>>> DeleteBookingAsync(int bookingId)
    {
        await _bookingService.DeleteBookingAsync(bookingId);
        return Ok(APIResponse<object>.Builder()
            .WithStatusCode(HttpStatusCode.NoContent)
            .WithSuccess(true)
            .Build());
    }

    [HttpGet("{bookingId:int}", Name = "GetBookingByIdAsync")]
    [Authorize]
    public async Task<ActionResult<APIResponse<BookingDTO>>> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _bookingService.GetBookingByIdAsync(bookingId);
        return Ok(APIResponse<BookingDTO>.Builder()
            .WithResult(booking)
            .WithSuccess(true)
            .Build());
    }

    [HttpGet]
    [Authorize(Roles = Constant.Role_Admin)]
    public async Task<ActionResult<APIResponse<List<BookingDTO>>>> GetAllBookingsAsync()
    {
        var bookings = await _bookingService.GetAllBookingsAsync();
        return Ok(APIResponse<List<BookingDTO>>.Builder()
            .WithResult(bookings)
            .WithSuccess(true)
            .Build());
    }

    [HttpPut("{bookingId:int}/cancel")]
    [Authorize]
    public async Task<ActionResult<APIResponse<object>>> CancelBookingAsync(int bookingId)
    {
        await _bookingService.CancelBookingAsync(bookingId);
        return Ok(APIResponse<object>.Builder()
            .WithSuccess(true)
            .WithMessage("Vé đã được hủy thành công. Nếu vé đã thanh toán, yêu cầu hoàn tiền sẽ được xử lý trong thời gian sớm nhất.")
            .WithStatusCode(HttpStatusCode.OK)
            .Build());
    }

    [HttpPut("{bookingId:int}")]
    [Authorize]
    public async Task<ActionResult<APIResponse<BookingDTO>>> UpdateBookingAsync(int bookingId, [FromBody] BookingUpdateDTO bookingUpdateDTO)
    {
        var booking = await _bookingService.UpdateBookingAsync(bookingUpdateDTO);
        return Ok(APIResponse<BookingDTO>.Builder()
            .WithResult(booking)
            .WithSuccess(true)
            .Build());
    }

    [HttpGet("my-bookings")]
    [Authorize]
    public async Task<ActionResult<APIResponse<List<MyBookingDTO>>>> GetMyBookingsAsync()
    {
        var bookings = await _bookingService.GetMyBookingsAsync();
        return Ok(APIResponse<List<MyBookingDTO>>.Builder()
            .WithResult(bookings)
            .WithSuccess(true)
            .Build());
    }
}