using FlightReservation.Infrastructure.Cache;
using FlightReservation.Infrastructure.Email;
using FlightReservation.Infrastructure.Hangfire.Jobs;
using FlightReservation.Infrastructure.Messaging.Consumers;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FlightReservation.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddRedis(config);
        services.AddEmailService(config);
        services.AddHangfireServices(config);
        services.AddMessaging(config);
        return services;
    }

    private static void AddRedis(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Redis")!;
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        services.AddSingleton<ICacheService, RedisCacheService>();
    }

    private static void AddEmailService(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SmtpSettings>(config.GetSection("Smtp"));
        services.AddTransient<IEmailService, SmtpEmailService>();
    }

    private static void AddHangfireServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(config.GetConnectionString("DefaultConnection")));

        services.AddHangfireServer(opt =>
        {
            opt.WorkerCount = 5;
            opt.Queues = ["critical", "default", "low"];
        });

        services.AddTransient<MarkCompletedFlightsJob>();
        services.AddTransient<FlightReminderJob>();
        services.AddTransient<CleanupAuditLogsJob>();
    }

    private static void AddMessaging(this IServiceCollection services, IConfiguration config)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReservationCreatedConsumer>();
            x.AddConsumer<ReservationCancelledConsumer>();
            x.AddConsumer<UserRegisteredConsumer>();
            x.AddConsumer<FlightCancelledConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(config["RabbitMq:Host"], config["RabbitMq:VHost"], h =>
                {
                    h.Username(config["RabbitMq:UserName"]!);
                    h.Password(config["RabbitMq:Password"]!);
                });

                cfg.UseMessageRetry(r => r.Exponential(3,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(5)));

                cfg.ConfigureEndpoints(ctx);
            });
        });
    }
}
