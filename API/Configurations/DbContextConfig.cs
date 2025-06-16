using API.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Configurations
{
    public static class DbContextConfig
    {
        public static void AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
