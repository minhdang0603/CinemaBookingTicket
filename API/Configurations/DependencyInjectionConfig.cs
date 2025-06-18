using API.Data.DbInitializer;
using API.Repositories.IRepositories;
using API.Repositories;
using API.Services.IServices;
using API.Services;

namespace API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddTransient<IEmailService, BrevoEmailService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<ITheaterService, TheaterService>();
            services.AddScoped<IScreenService, ScreenService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IShowTimeService, ShowTimeService>();
        }
    }
}
