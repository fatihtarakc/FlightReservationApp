namespace App.Cache.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(ICacheService<>), typeof(CacheService<>));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = (configuration.GetSection(ConnectionOptions.Connections).Get<ConnectionOptions>())?.Redis;
                options.InstanceName = "FlightReservationApp:";
            });

            return services;
        }
    }
}