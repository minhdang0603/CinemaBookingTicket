using Microsoft.AspNetCore.Authentication.Cookies;
using Web.Configurations;
using Web.Middlewares;
using Web.Services;
using Web.Services.IServices;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
			});

            // Đăng ký AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingConfig));

            // Đăng ký Session
            builder.Services.AddAuthenticationConfiguration();

            // Đăng ký HttpClient
            builder.Services.AddHttpClientConfiguration(builder.Configuration);

            // Đăng ký các dịch vụ
            builder.Services.AddDependencyInjectionConfiguration();

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
            app.UseSession();

            app.UseAuthentication();
            // Đăng ký middleware đồng bộ token vào session
            app.UseMiddleware<TokenSyncMiddleware>();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Public}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}