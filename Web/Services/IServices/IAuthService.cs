using Web.Models.DTOs.Request;

namespace Web.Services.IServices
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDTO loginRequest);
        Task LogoutAsync();
        Task<T> RegisterAsync<T>(UserCreateDTO registerRequest);
        Task<T> VerifyEmailAsync<T>(string userId, string token);
        Task<T> ForgotPasswordAsync<T>(ForgotPasswordRequestDTO request);
        Task<T> ResetPasswordAsync<T>(ResetPasswordRequestDTO request);
        // Task<bool> IsUserAuthenticatedAsync();
        // Task<string> GetCurrentUserNameAsync();
        // Task<string> GetCurrentUserRoleAsync();
    }
}
