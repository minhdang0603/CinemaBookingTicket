using Microsoft.AspNetCore.Authentication.Cookies;
using Web.Services;
using Web.Services.IServices;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(options =>
              {
                  options.Cookie.HttpOnly = true;
                  options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                  options.LoginPath = "/Public/Auth/Login";
                  options.LogoutPath = "/Public/Auth/Logout";
                  options.AccessDeniedPath = "/Public/Auth/AccessDenied";
                  options.SlidingExpiration = true;
              });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(100);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            // Đăng ký HttpClient
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient("CinemaBookingTicketAPI", c =>
            {
                string apiUrl = builder.Configuration["ServiceUrls:CinemaBookingTicketAPI"] ?? "http://localhost:5000";
                c.BaseAddress = new Uri(apiUrl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Public}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}