using Microsoft.AspNetCore.Authentication.Cookies;

namespace Web.Configurations
{
    public static class AuthenticationConfig
    {
        public static void AddAuthenticationConfiguration(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(options =>
              {
                  options.Cookie.HttpOnly = true;
                  options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                  options.LoginPath = "/Public/Auth/Login";
                  options.LogoutPath = "/Public/Auth/Logout";
                  options.AccessDeniedPath = "/Public/Auth/AccessDenied";
                  options.SlidingExpiration = true;
              });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(100);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }
    }
}
