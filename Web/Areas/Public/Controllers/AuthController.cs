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
        public async Task<IActionResult> Login(LoginRequestDTO requestDTO)
        {
            if (!ModelState.IsValid)
            {
                requestDTO.Password = string.Empty;
                return View(requestDTO);
            }

            var response = await _authService.LoginAsync<APIResponse>(requestDTO);

            if (response != null && response.IsSuccess)
            {
                LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(model.Token);

                _logger.LogInformation($"User {jwt.Claims.FirstOrDefault(u => u.Type == "unique_name")?.Value} logged in successfully.");

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
                // Lưu token vào claim để đồng bộ lại session khi cần
                identity.AddClaim(new Claim("access_token", model.Token));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString(Constant.SessionToken, model.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
                requestDTO.Password = string.Empty;
                return View(requestDTO);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(Constant.SessionToken, "");
            return RedirectToAction("Index", "Home");
        }
    }
}
