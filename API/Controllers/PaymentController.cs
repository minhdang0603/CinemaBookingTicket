using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

    [HttpGet("vnpay-return")]
    public async Task<ActionResult> VNPayReturn([FromQuery] IQueryCollection queryParams)
    {
        try
        {
            var response = await _paymentService.ProcessVNPayReturnAsync(queryParams);

            // Log the redirect URL for debugging
            _logger.LogInformation($"Redirecting to URL: {response.RedirectUrl}");

            // Chuyển hướng về trang web dựa trên kết quả thanh toán
            if (string.IsNullOrEmpty(response.RedirectUrl))
            {
                return Redirect($"{_frontendUrl}/payment-error");
            }

            return Redirect(response.RedirectUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VNPay return");
            return Redirect($"{_frontendUrl}/payment-error");
        }
    }

    [HttpPost("vnpay-ipn")]
    public async Task<ActionResult<VNPayIPNResponseDTO>> VNPayIPN([FromQuery] IQueryCollection queryParams)
    {
        try
        {
            var response = await _paymentService.ProcessVNPayIPNAsync(queryParams);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VNPay IPN");
            return StatusCode(500, new VNPayIPNResponseDTO
            {
                RspCode = "99",
                Message = "Error processing payment notification"
            });
        }
    }
}