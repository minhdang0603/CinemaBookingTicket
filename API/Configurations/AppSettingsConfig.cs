namespace API.Configurations
{
    public static class AppSettingsConfig
    {
        public static IConfigurationBuilder ConfigureAppSettings(this IConfigurationBuilder builder, WebApplicationBuilder webBuilder)
        {
           return builder
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .AddJsonFile($"appsettings.{webBuilder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
             .AddEnvironmentVariables();
        }
    }
}
