using System.Text;
using API.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using brevo_csharp.Client;
using API.Services;
using API.Services.IServices;
using API.Repositories;
using API.Repositories.IRepositories;
using API.Middlewares;
using API.Exceptions;
using Microsoft.AspNetCore.Authorization;
using API.DTOs;
using System.Net;
using API.Data.DbInitializer;
using Utility;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration.Default.ApiKey.Add("api-key", builder.Configuration.GetValue<string>("BrevoApi:ApiKey"));

            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
            });


            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            builder.Services.AddAutoMapper(typeof(MappingConfig));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Identity services
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            builder.Services.AddTransient<IEmailService, BrevoEmailService>();

            builder.Services.AddScoped<IShowTimeRepository, ShowTimeRepository>();

            builder.Services.AddScoped<IShowTimeService, ShowTimeService>();

            builder.Services.AddResponseCaching();

            var key = builder.Configuration.GetValue<string>("JwtSettings:Secret");

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(option =>
                {
                    option.RequireHttpsMetadata = false;
                    option.SaveToken = true;
                    option.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        //ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:ValidIssuer"),
                        //ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:ValidAudience")
                    };

                    option.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.HttpContext.Response.ContentType = "application/json";
                            var response = APIResponse<object>.Builder()
                                .WithStatusCode(HttpStatusCode.Unauthorized)
                                .WithErrorMessages("Authentication failed")
                                .WithSuccess(false)
                                .Build();
                            return context.HttpContext.Response.WriteAsJsonAsync(response);
                        },
                        OnForbidden = context =>
                        {
                            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.HttpContext.Response.ContentType = "application/json";
                            var response = APIResponse<object>.Builder()
                                .WithStatusCode(HttpStatusCode.Forbidden)
                                .WithErrorMessages("You do not have permission to access this resource.")
                                .WithSuccess(false)
                                .Build();
                            return context.HttpContext.Response.WriteAsJsonAsync(response);
                        }
                    };

                });


            builder.Services.AddControllers(options =>
            {
                options.CacheProfiles.Add("Default",
                    new Microsoft.AspNetCore.Mvc.CacheProfile
                    {
                        Duration = 60 // Cache for 60 seconds
                    });
            }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseExceptionHandler();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            SeedDatabase();

            app.MapControllers();

            app.MapGet("/", () => { throw new AppException(ErrorCodes.UserAlreadyExists("email")); });

            app.Run();

            void SeedDatabase()
            {
                using (var scope = app.Services.CreateScope())
                {
                    scope.ServiceProvider.GetRequiredService<IDbInitializer>().Initialize();
                }
            }
        }
        
    }
}
