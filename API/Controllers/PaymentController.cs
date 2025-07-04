using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using Utility;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _frontendUrl;

    public PaymentController(
        IPaymentService paymentService,
        ILogger<PaymentController> logger,
        IConfiguration configuration)
    {
        _paymentService = paymentService;
        _logger = logger;
        _configuration = configuration;
        _frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5082";
    }

    [HttpPost("create-vnpay-payment")]
    [Authorize]
    public async Task<ActionResult<APIResponse<string>>> CreateVNPayPayment([FromBody] VNPayRequestDTO request)
    {
        if (request == null)
        {
            return BadRequest(APIResponse<string>.Builder()
                .WithSuccess(false)
                .WithErrorMessages(new List<string> { "Invalid payment request" })
                .Build());
        }

        _logger.LogInformation("Creating VNPay payment URL for booking ID: {BookingId}, Amount: {Amount}", 
            request.BookingId, request.Amount);

        var paymentUrl = await _paymentService.CreateVNPayPaymentUrl(request);
        
        return Ok(APIResponse<string>.Builder()
                .WithStatusCode(HttpStatusCode.OK)
                .WithSuccess(true)
                .WithResult(paymentUrl)
                .Build());
    }

    [HttpGet("vnpay-check")]
    [AllowAnonymous]
    public async Task<ActionResult<APIResponse<VNPayResponseDTO>>> VNPayCheck([FromQuery] IQueryCollection queryParams)
    {
        _logger.LogInformation("Processing VNPay return with parameters: {@Params}",
            queryParams.ToDictionary(x => x.Key, x => x.Value.ToString()));
        
        var response = await _paymentService.ProcessVNPayReturnAsync(queryParams);

        _logger.LogInformation("VNPay payment processed: Success={Success}, OrderId={OrderId}, TransactionId={TransactionId}", 
            response.Success, response.OrderId, response.TransactionId);
        
        return Ok(APIResponse<VNPayResponseDTO>.Builder()
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(response.Success)
            .WithResult(response)
            .WithErrorMessages(response.Success ? null : new List<string> { response.Message })
            .Build());
    }

    [HttpPost("vnpay-ipn")]
    [AllowAnonymous]
    public async Task<ActionResult<APIResponse<VNPayIPNResponseDTO>>> VNPayIPN([FromQuery] IQueryCollection queryParams)
    {
        _logger.LogInformation("Received VNPay IPN callback with parameters: {@Params}",
            queryParams.ToDictionary(x => x.Key, x => x.Value.ToString()));
            
        var response = await _paymentService.ProcessVNPayIPNAsync(queryParams);
        
        _logger.LogInformation("VNPay IPN processed: RspCode={RspCode}, Message={Message}", 
            response.RspCode, response.Message);
            
        return Ok(APIResponse<VNPayIPNResponseDTO>.Builder()
                    .WithResult(response)
                    .WithSuccess(true)
                    .Build());
    }

    [HttpGet("by-booking/{bookingId}")]
    [Authorize]
    public async Task<ActionResult<APIResponse<PaymentDTO>>> GetPaymentByBookingId(int bookingId)
    {
        _logger.LogInformation("Getting payment for booking ID: {BookingId}", bookingId);
        
        var payment = await _paymentService.GetPaymentsByBookingIdAsync(bookingId);
        
        return Ok(APIResponse<PaymentDTO>.Builder()
            .WithStatusCode(HttpStatusCode.OK)
            .WithResult(payment)
            .WithSuccess(true)
            .Build());
    }

    [HttpGet("payment-status/{paymentId}")]
    [AllowAnonymous]
    public async Task<ActionResult<APIResponse<PaymentDTO>>> GetPaymentStatus(string paymentId)
    {
        _logger.LogInformation("Getting payment status for payment ID: {PaymentId}", paymentId);
        
        if (!int.TryParse(paymentId, out int id))
        {
            return BadRequest(APIResponse<object>.Builder()
                .WithSuccess(false)
                .WithErrorMessages(new List<string> { "Invalid payment ID format" })
                .Build());
        }
        
        // Để frontend có thể kiểm tra trạng thái thanh toán, ta có thể triển khai phương thức
        // GetPaymentByIdAsync trong IPaymentService. Hiện tại sử dụng phương pháp khác để mô phỏng.
        
        // Truy vấn trực tiếp payment qua unit of work
        var payment = await _paymentService.UpdatePaymentStatusAsync(id, null);
        
        return Ok(APIResponse<PaymentDTO>.Builder()
            .WithStatusCode(HttpStatusCode.OK)
            .WithResult(payment)
            .WithSuccess(true)
            .Build());
    }
}