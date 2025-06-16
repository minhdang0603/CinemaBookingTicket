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
        }
    }
}
