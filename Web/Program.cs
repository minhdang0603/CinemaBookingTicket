namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

<<<<<<< Updated upstream
=======
            // Đăng ký AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingConfig));


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
            builder.Services.AddScoped<IScreenService, ScreenService>();
            builder.Services.AddScoped<ITheaterService, TheaterService>();
            builder.Services.AddScoped<IProvinceService, ProvinceService>();

>>>>>>> Stashed changes
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
