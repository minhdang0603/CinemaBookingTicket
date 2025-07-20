using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Utility;

namespace Web.Middlewares
{
    public class TokenSyncMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenSyncMiddleware> _logger;

        public TokenSyncMiddleware(RequestDelegate next, ILogger<TokenSyncMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Kiểm tra nếu người dùng đã xác thực
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Trường hợp 1: Kiểm tra token trong cookies
                if (!context.Request.Cookies.TryGetValue(Constant.AccessToken, out string? cookieToken) || string.IsNullOrEmpty(cookieToken))
                {
                    // Không có token trong cookies, thử lấy từ claims
                    var tokenClaim = context.User.FindFirst(Constant.AccessToken);
                    if (tokenClaim != null && !string.IsNullOrEmpty(tokenClaim.Value))
                    {
                        // Kiểm tra tính hợp lệ của token claim
                        try
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var jwt = handler.ReadJwtToken(tokenClaim.Value);

                            // Nếu token đã hết hạn, đăng xuất người dùng
                            if (jwt.ValidTo < DateTime.UtcNow)
                            {
                                _logger.LogInformation("Token from claims is expired. Logging out user.");
                                await SignOutUserAsync(context);
                                await _next(context);
                                return;
                            }

                            // Token hợp lệ, lưu vào cả cookies và session
                            var cookieOptions = new CookieOptions
                            {
                                Expires = jwt.ValidTo,
                                HttpOnly = true,
                                Secure = context.Request.IsHttps,
                                SameSite = SameSiteMode.Lax
                            };
                            context.Response.Cookies.Append(Constant.AccessToken, tokenClaim.Value, cookieOptions);
                            _logger.LogInformation("Token restored from claims to cookies and session.");
                        }
                        catch (Exception ex)
                        {
                            // Token không hợp lệ, đăng xuất người dùng
                            _logger.LogWarning(ex, "Invalid token format in claims. Logging out user.");
                            await SignOutUserAsync(context);
                            await _next(context);
                            return;
                        }
                    }
                    else
                    {
                        // Không có token trong claims, đăng xuất người dùng
                        _logger.LogWarning("Authenticated user without token in cookies or claims. Logging out.");
                        await SignOutUserAsync(context);
                        await _next(context);
                        return;
                    }
                }
                else
                {
                    // Có token trong cookies, kiểm tra tính hợp lệ
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwt = handler.ReadJwtToken(cookieToken);

                        // Kiểm tra thời gian hết hạn
                        if (jwt.ValidTo < DateTime.UtcNow)
                        {
                            _logger.LogInformation("Token from cookies is expired. Logging out user.");
                            await SignOutUserAsync(context);
                            await _next(context);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Token không hợp lệ, đăng xuất người dùng
                        _logger.LogWarning(ex, "Invalid token format in cookies. Logging out user.");
                        await SignOutUserAsync(context);
                        await _next(context);
                        return;
                    }
                }
            }

            await _next(context);
        }

        private async Task SignOutUserAsync(HttpContext context)
        {
            // Đăng xuất người dùng
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Xóa cookie token
            context.Response.Cookies.Delete(Constant.AccessToken);
        }
    }

    public static class TokenSyncMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenSync(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenSyncMiddleware>();
        }
    }
}
