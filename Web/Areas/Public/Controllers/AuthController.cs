using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Models.ViewModels;
using Web.Services.IServices;

namespace Web.Areas.Public.Controllers
{
    [Area("Public")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                return View(model);
            }

            // Convert ViewModel to DTO
            var userCreateDTO = new UserCreateDTO
            {
                Email = model.Email,
                Password = model.Password,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber
            };

            var response = await _authService.RegisterAsync<APIResponse>(userCreateDTO);

            if (response != null && response.IsSuccess)
            {
                // Registration successful - redirect to login with success message
                _logger.LogInformation($"User {model.Email} registered successfully.");
                TempData["success"] = "Registration successful! Please check your email to verify your account before logging in.";
                return RedirectToAction(nameof(Login));
            }
            else
            {
                TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Registration failed. Please try again.";
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO requestDTO)
        {
            if (!ModelState.IsValid)
            {
                requestDTO.Password = string.Empty;
                return View(requestDTO);
            }

            var response = await _authService.LoginAsync<APIResponse>(requestDTO);

            if (response != null && response.IsSuccess && response.Result != null)
            {
                var resultStr = Convert.ToString(response.Result);
                if (string.IsNullOrEmpty(resultStr))
                {
                    TempData["error"] = "Login failed. Invalid response from server.";
                    requestDTO.Password = string.Empty;
                    return View(requestDTO);
                }

                var loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(resultStr);
                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    TempData["error"] = "Login failed. Invalid token received.";
                    requestDTO.Password = string.Empty;
                    return View(requestDTO);
                }

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(loginResponse.Token);

                var uniqueNameClaim = jwt.Claims.FirstOrDefault(u => u.Type == "unique_name");
                _logger.LogInformation($"User {uniqueNameClaim?.Value} logged in successfully.");

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                if (uniqueNameClaim != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Name, uniqueNameClaim.Value));
                }

                var roleClaim = jwt.Claims.FirstOrDefault(u => u.Type == "role");
                if (roleClaim != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }

                // Lưu token vào claim để đồng bộ lại session khi cần
                identity.AddClaim(new Claim("access_token", loginResponse.Token));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Add token to cookies with expiration matching the JWT expiration
                var cookieOptions = new CookieOptions
                {
                    Expires = jwt.ValidTo,
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax
                };
                Response.Cookies.Append(Constant.AccessToken, loginResponse.Token, cookieOptions);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Login failed. Please try again.";
                requestDTO.Password = string.Empty;
                return View(requestDTO);
            }
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(Constant.SessionToken, "");

            // Clear the authentication cookie
            Response.Cookies.Delete(Constant.AccessToken);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CheckTokenStatus()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Json(new { isAuthenticated = false });
            }

            // Kiểm tra token trong cookies
            if (!Request.Cookies.TryGetValue(Constant.AccessToken, out string? token) || string.IsNullOrEmpty(token))
            {
                return Json(new { isAuthenticated = false, reason = "token_not_found" });
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                // Kiểm tra thời gian hết hạn
                if (jwt.ValidTo < DateTime.UtcNow)
                {
                    return Json(new { isAuthenticated = false, reason = "token_expired" });
                }

                return Json(new { isAuthenticated = true });
            }
            catch (Exception)
            {
                return Json(new { isAuthenticated = false, reason = "token_invalid" });
            }
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["error"] = "Invalid verification link.";
                return RedirectToAction(nameof(Login));
            }

            var response = await _authService.VerifyEmailAsync<APIResponse>(userId, token);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Email verified successfully! You can now log in.";
                _logger.LogInformation("Email verification successful for userId: {UserId}", userId);
            }
            else
            {
                TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Email verification failed. The link may be invalid or expired.";
                _logger.LogWarning("Email verification failed for userId: {UserId}", userId);
            }

            return RedirectToAction(nameof(Login));
        }
    }
}
