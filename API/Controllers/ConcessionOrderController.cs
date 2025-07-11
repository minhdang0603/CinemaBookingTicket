using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utility;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConcessionOrderController : ControllerBase
{
    private readonly IConcessionOrderService _concessionOrderService;

    public ConcessionOrderController(IConcessionOrderService concessionOrderService)
    {
        _concessionOrderService = concessionOrderService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<APIResponse<ConcessionOrderDTO>>> CreateConcessionOrderAsync([FromBody] ConcessionOrderCreateDTO concessionOrderCreateDTO)
    {
        var concessionOrder = await _concessionOrderService.CreateConcessionOrderAsync(concessionOrderCreateDTO);
        return CreatedAtRoute("GetConcessionOrderByIdAsync", new { concessionOrderId = concessionOrder.Id }, APIResponse<ConcessionOrderDTO>.Builder()
            .WithResult(concessionOrder)
            .WithStatusCode(HttpStatusCode.Created)
            .WithSuccess(true)
            .Build());
    }

    [HttpDelete("{concessionOrderId:int}")]
    [Authorize]
    public async Task<ActionResult<APIResponse<object>>> DeleteConcessionOrderAsync(int concessionOrderId)
    {
        await _concessionOrderService.DeleteConcessionOrderAsync(concessionOrderId);
        return Ok(APIResponse<object>.Builder()
            .WithStatusCode(HttpStatusCode.NoContent)
            .WithSuccess(true)
            .Build());
    }

    [HttpGet("{concessionOrderId:int}", Name = "GetConcessionOrderByIdAsync")]
    [Authorize]
    public async Task<ActionResult<APIResponse<ConcessionOrderDTO>>> GetConcessionOrderByIdAsync(int concessionOrderId)
    {
        var concessionOrder = await _concessionOrderService.GetConcessionOrderByIdAsync(concessionOrderId);
        return Ok(APIResponse<ConcessionOrderDTO>.Builder()
            .WithResult(concessionOrder)
            .WithSuccess(true)
            .Build());
    }

    [HttpGet]
    [Authorize(Roles = Constant.Role_Admin)]
    public async Task<ActionResult<APIResponse<List<ConcessionOrderDTO>>>> GetAllConcessionOrdersAsync()
    {
        var concessionOrders = await _concessionOrderService.GetAllConcessionOrdersAsync();
        return Ok(APIResponse<List<ConcessionOrderDTO>>.Builder()
            .WithResult(concessionOrders)
            .WithSuccess(true)
            .Build());
    }

    [HttpGet("by-booking/{bookingId:int}")]
    [Authorize]
    public async Task<ActionResult<APIResponse<List<ConcessionOrderDTO>>>> GetConcessionOrdersByBookingIdAsync(int bookingId)
    {
        var concessionOrders = await _concessionOrderService.GetConcessionOrdersByBookingIdAsync(bookingId);
        return Ok(APIResponse<List<ConcessionOrderDTO>>.Builder()
            .WithResult(concessionOrders)
            .WithSuccess(true)
            .Build());
    }
}
