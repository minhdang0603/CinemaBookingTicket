using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Utility;

namespace Web.Middlewares
{
    public class TokenSyncMiddleware
    {
        private readonly RequestDelegate _next;
        public TokenSyncMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated && string.IsNullOrEmpty(context.Session.GetString(Constant.SessionToken)))
            {
                var tokenClaim = context.User.FindFirst("access_token");
                if (tokenClaim != null)
                {
                    context.Session.SetString(Constant.SessionToken, tokenClaim.Value);
                }
            }
            await _next(context);
        }
    }
}
