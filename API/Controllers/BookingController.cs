using API.Data.Models;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<APIResponse<string>>> CreateBookingAsync([FromBody] BookingCreateDTO bookingCreateDTO)
    {
        var paymentUrl = await _bookingService.CreateBookingWithPaymentAsync(bookingCreateDTO);
        return Ok(APIResponse<string>.Builder()
            .WithResult(paymentUrl)
            .WithSuccess(true)
            .Build());
    }

    [HttpDelete("{bookingId:int}")]
    [Authorize(Roles = Constant.Role_Admin)]
    public async Task<ActionResult<APIResponse<object>>> DeleteBookingAsync(int bookingId)
    {
        await _bookingService.DeleteBookingAsync(bookingId);
        return Ok(APIResponse<object>.Builder()
            .WithSuccess(true)
            .Build());
    }

    [HttpGet("{bookingId:int}")]
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
            .Build());
    }

    [HttpGet("my-bookings")]
    [Authorize]
    public async Task<ActionResult<APIResponse<List<BookingDTO>>>> GetMyBookingsAsync()
    {
        var bookings = await _bookingService.GetMyBookingsAsync();
        return Ok(APIResponse<List<BookingDTO>>.Builder()
            .WithResult(bookings)
            .WithSuccess(true)
            .Build());
    }
}