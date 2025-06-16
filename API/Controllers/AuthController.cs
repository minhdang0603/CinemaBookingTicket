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
    public async Task<ActionResult<APIResponse<LoginResponseDTO>>> Register([FromBody] UserCreateDTO userCreateDTO)
    {
        var response = await _authService.RegisterAsync(userCreateDTO);

        return CreatedAtAction(nameof(Login), new { email = userCreateDTO.Email }, APIResponse<LoginResponseDTO>.Builder()
            .WithResult(response)
            .WithStatusCode(HttpStatusCode.Created)
            .WithSuccess(true)
            .Build());
    }

}
