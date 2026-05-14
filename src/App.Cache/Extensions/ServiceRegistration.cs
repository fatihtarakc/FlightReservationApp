using App.Cache.Services.Abstract;
using App.Cache.Services.Concrete;
using Microsoft.Extensions.Configuration;

namespace App.Cache.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionOptions = configuration.GetSection(ConnectionOptions.Connections).Get<ConnectionOptions>();

            services.AddStackExchangeRedisCache(opts =>
            {
                opts.Configuration = connectionOptions?.Redis;
                opts.InstanceName = "FlightReservation:";
            });

            services.AddScoped(typeof(ICacheService<>), typeof(CacheService<>));

            return services;
        }
    }
}
