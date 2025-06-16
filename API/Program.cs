using API.Configurations;
using API.Data.DbInitializer;
using API.Exceptions;
using API.Middlewares;
using API.Services.IServices;
using API.Services;
using brevo_csharp.Client;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Appsettings Config
            builder.Configuration.ConfigureAppSettings(builder);

            Configuration.Default.ApiKey.Add("api-key", builder.Configuration.GetValue<string>("BrevoApi:ApiKey"));

            // Add services to the container.
            builder.Services.AddControllers();


            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            builder.Services.AddAutoMapper(typeof(MappingConfig));

            // Add DbContext Config
            builder.Services.AddDbContextConfiguration(builder.Configuration);

            // Register Identity services
            builder.Services.AddIdentityConfig();

            // Add Dependency Injection Config
            builder.Services.AddDependencyInjectionConfiguration();

            builder.Services.AddResponseCaching();

            // Add Authentication Config
            builder.Services.ConfigureAuthentication(builder.Configuration);

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
