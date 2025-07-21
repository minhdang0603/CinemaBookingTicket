using System.Net;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<APIResponse<LoginResponseDTO>>> Login([FromBody] LoginRequestDTO loginRequest)
    {
        var response = await _authService.LoginAsync(loginRequest);

        return Ok(APIResponse<LoginResponseDTO>.Builder()
            .WithResult(response)
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .Build());
    }

    [HttpPost("register")]
    public async Task<ActionResult<APIResponse<string>>> Register([FromBody] UserCreateDTO userCreateDTO)
    {
        var response = await _authService.RegisterAsync(userCreateDTO);

        return CreatedAtAction(nameof(Login), new { email = userCreateDTO.Email }, APIResponse<string>.Builder()
            .WithResult(response)
            .WithStatusCode(HttpStatusCode.Created)
            .WithSuccess(true)
            .Build());
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<APIResponse<string>>> VerifyEmail([FromQuery] string userId, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return BadRequest(APIResponse<string>.Builder()
                .WithErrorMessages(new List<string> { "UserId and token are required." })
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithSuccess(false)
                .Build());
        }

        var isVerified = await _authService.VerifyEmailAsync(userId, token);

        if (!isVerified)
        {
            return BadRequest(APIResponse<string>.Builder()
                .WithErrorMessages(new List<string> { "Invalid or expired verification token." })
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithSuccess(false)
                .Build());
        }

        return Ok(APIResponse<string>.Builder()
            .WithResult("Email verified successfully. You can now log in.")
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .Build());
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<APIResponse<string>>> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
    {
        var response = await _authService.ForgotPasswordAsync(request);

        return Ok(APIResponse<string>.Builder()
            .WithResult(response)
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .Build());
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<APIResponse<string>>> ResetPassword([FromBody] ResetPasswordRequestDTO request)
    {
        var isReset = await _authService.ResetPasswordAsync(request);

        if (!isReset)
        {
            return BadRequest(APIResponse<string>.Builder()
                .WithErrorMessages(new List<string> { "Invalid or expired reset token, or password reset failed." })
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithSuccess(false)
                .Build());
        }

        return Ok(APIResponse<string>.Builder()
            .WithResult("Password reset successfully. You can now log in with your new password.")
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .Build());
    }

}
