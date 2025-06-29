using Web.Services;
using Web.Services.IServices;

namespace Web.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IScreenService, ScreenService>();
            services.AddScoped<ITheaterService, TheaterService>();
            services.AddScoped<IShowtimeService, ShowtimeService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IConcessionCategoryService, ConcessionCategoryService>();
            services.AddScoped<IConcessionService, ConcessionService>();
            services.AddScoped<IProvinceService, ProvinceService>();
        }
    }
}
