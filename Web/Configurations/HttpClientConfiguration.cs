using Microsoft.Extensions.DependencyInjection;

namespace Web.Configurations
{
    public static class HttpClientConfiguration
    {
        public static IServiceCollection AddHttpClientConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddHttpClient("CinemaBookingTicketAPI", c =>
            {
                string apiUrl = configuration["ServiceUrls:CinemaBookingTicketAPI"] ?? "http://localhost:5000";
                c.BaseAddress = new Uri(apiUrl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            return services;
        }
    }
}
