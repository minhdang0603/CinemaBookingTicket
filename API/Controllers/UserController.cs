using System.Net;
using System.Security.Claims;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = Constant.Role_Admin)]
    public async Task<ActionResult<APIResponse<UserDTO>>> GetUserDetails(string userId)
    {
        var user = await _userService.GetUserDetailsAsync(userId);

        return Ok(APIResponse<UserDTO>.Builder()
            .WithResult(user)
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .Build());
    }

    [HttpGet("profile")]
    public async Task<ActionResult<APIResponse<UserDTO>>> GetProfile()
    {
        var user = await _userService.GetProfile();

        return Ok(APIResponse<UserDTO>.Builder()
            .WithResult(user)
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .Build());
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Constant.Role_Admin)]
    public async Task<ActionResult<APIResponse<UserDTO>>> UpdateUserDetails(Guid id, [FromBody] UserUpdateDTO updateUserRequest)
    {
        if (id.ToString() != updateUserRequest.UserId)
        {
            return BadRequest(APIResponse<UserDTO>.Builder()
                .WithErrorMessages(new List<string> { "User ID mismatch." })
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithSuccess(false)
                .Build());
        }

        var updatedUser = await _userService.UpdateUserDetailsAsync(updateUserRequest);

        return Ok(APIResponse<UserDTO>.Builder()
            .WithResult(updatedUser)
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .Build());
    }

    [HttpPut("profile")]
    public async Task<ActionResult<APIResponse<UserDTO>>> UpdateProfile([FromBody] UserUpdateDTO updateUserRequest)
    {
        // Get current user ID from claims
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? throw new AppException(ErrorCodes.UnauthorizedAccess());

        // Ensure the user ID in the request matches the current user's ID
        updateUserRequest.UserId = userId;

        var updatedUser = await _userService.UpdateUserDetailsAsync(updateUserRequest);

        return Ok(APIResponse<UserDTO>.Builder()
            .WithResult(updatedUser)
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .Build());
    }


    [HttpGet]
    [Authorize(Roles = Constant.Role_Admin)]
    public async Task<ActionResult<APIResponse<List<UserDTO>>>> GetAllUsers(int pageNumber, int pageSize)
    {
        var users = await _userService.GetAllUsersAsync(pageNumber, pageSize);
        return Ok(APIResponse<List<UserDTO>>.Builder()
            .WithResult(users)
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .Build());
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = Constant.Role_Admin)]
    public async Task<ActionResult<APIResponse<object>>> DeleteUser(string userId)
    {
        await _userService.DeleteUserAsync(userId);
        return Ok(APIResponse<object>.Builder()
            .WithStatusCode(HttpStatusCode.NoContent)
            .WithSuccess(true)
            .Build());
    }
}