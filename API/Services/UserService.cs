using System.Security.Claims;
using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace API.Services;

public class UserService : IUserService
{

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new AppException(ErrorCodes.EntityNotFound("User", userId));

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new AppException(ErrorCodes.InternalServerError());
        }
    }

    public async Task<List<UserDTO>> GetAllUsersAsync(int pageNumber, int pageSize)
    {
        var users = await _userManager.Users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<List<UserDTO>>(users);
    }

    public async Task<UserDTO> GetProfile()
    {
        // Get current user ID from claims
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? throw new AppException(ErrorCodes.UnauthorizedAccess());

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId)
                            ?? throw new AppException(ErrorCodes.EntityNotFound("User", userId));

        return _mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO> GetUserDetailsAsync(string userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId)
                            ?? throw new AppException(ErrorCodes.EntityNotFound("User", userId));
        return _mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO> UpdateUserDetailsAsync(UserUpdateDTO updateUserRequest)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == updateUserRequest.UserId)
                            ?? throw new AppException(ErrorCodes.EntityNotFound("User", updateUserRequest.UserId));

        _mapper.Map(updateUserRequest, user);
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new AppException(ErrorCodes.InternalServerError());
        }

        return _mapper.Map<UserDTO>(user);
    }
}