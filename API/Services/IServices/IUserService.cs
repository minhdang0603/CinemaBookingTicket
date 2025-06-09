using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IUserService
{
    Task<UserDTO> GetUserDetailsAsync(string userId);
    Task<UserDTO> UpdateUserDetailsAsync(UserUpdateDTO updateUserRequest);
    Task<List<UserDTO>> GetAllUsersAsync(int pageNumber, int pageSize);
    Task DeleteUserAsync(string userId);
    Task<UserDTO> GetProfile();

}