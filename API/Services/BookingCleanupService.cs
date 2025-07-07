using API.Services.IServices;
using Utility;

namespace API.Services
{
    /// <summary>
    /// Background service to clean up expired bookings in the API
    /// Runs on a configurable interval and handles bookings that have expired
    /// </summary>
    public class BookingCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BookingCleanupService> _logger;
        private readonly int _cleanupIntervalMinutes;
        private readonly int _bookingExpiryMinutes;

        public BookingCleanupService(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILogger<BookingCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;

            // Get configuration values with defaults if not specified
            _cleanupIntervalMinutes = _configuration.GetValue<int>("Booking:CleanupIntervalMinutes", 1);
            _bookingExpiryMinutes = _configuration.GetValue<int>("Booking:ExpiryMinutes", 5);

            _logger.LogInformation("BookingCleanupService initialized with cleanup interval: {CleanupInterval} minutes, " +
                                   "booking expiry: {BookingExpiry} minutes",
                                   _cleanupIntervalMinutes, _bookingExpiryMinutes);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Booking Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Booking Cleanup Service running at: {time}", DateTimeOffset.Now);

                try
                {
                    await CleanupExpiredBookingsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up expired bookings.");
                }

                // Wait for the next cleanup interval
                await Task.Delay(TimeSpan.FromMinutes(_cleanupIntervalMinutes), stoppingToken);
            }

            _logger.LogInformation("Booking Cleanup Service is stopping.");
        }

        private async Task CleanupExpiredBookingsAsync()
        {
            // Create a scope to resolve scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    // Get the booking service using the service provider
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                    // Call the cleanup method
                    int deletedCount = await bookingService.CleanupExpiredBookingsAsync(_bookingExpiryMinutes);

                    if (deletedCount > 0)
                    {
                        _logger.LogInformation("Background service cleaned up {count} expired bookings", deletedCount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in CleanupExpiredBookingsAsync background process");
                }
            }
        }
    }
}
