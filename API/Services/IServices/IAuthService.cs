using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IAuthService
{
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest);
    Task<string> RegisterAsync(UserCreateDTO userCreateDTO);
    Task<bool> VerifyEmailAsync(string userId, string token);
}
