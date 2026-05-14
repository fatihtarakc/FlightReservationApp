using App.Queue.Consumers;
using App.Queue.Services.Concrete;

namespace App.Queue.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddQueueServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.EmailConfiguration));
            services.Configure<TwilioOptions>(configuration.GetSection(TwilioOptions.TwilioConfiguration));

            services.AddTransient<IEmailSenderService, MailKitEmailSenderService>();
            services.AddTransient<ISmsSenderService, TwilioSmsSenderService>();

            var rabbitMq = configuration.GetSection(RabbitMqOptions.RabbitMqConfiguration).Get<RabbitMqOptions>()
                ?? new RabbitMqOptions();

            services.AddMassTransit(configure =>
            {
                configure.AddConsumer<UserRegisteredConsumer>();
                configure.AddConsumer<BookingConfirmedConsumer>();
                configure.AddConsumer<BookingCancelledConsumer>();
                configure.AddConsumer<FlightCancelledConsumer>();
                configure.AddConsumer<FlightReminderConsumer>();
                configure.AddConsumer<VerificationCodeConsumer>();

                configure.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(rabbitMq.Host, (ushort)rabbitMq.Port, rabbitMq.VirtualHost, host =>
                    {
                        host.Username(rabbitMq.Username);
                        host.Password(rabbitMq.Password);
                    });

                    cfg.ReceiveEndpoint(QueueNames.UserRegistered, endpoint =>
                    {
                        endpoint.PrefetchCount = 16;
                        endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                        endpoint.ConfigureConsumer<UserRegisteredConsumer>(ctx);
                    });

                    cfg.ReceiveEndpoint(QueueNames.BookingConfirmed, endpoint =>
                    {
                        endpoint.PrefetchCount = 16;
                        endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                        endpoint.ConfigureConsumer<BookingConfirmedConsumer>(ctx);
                    });

                    cfg.ReceiveEndpoint(QueueNames.BookingCancelled, endpoint =>
                    {
                        endpoint.PrefetchCount = 16;
                        endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                        endpoint.ConfigureConsumer<BookingCancelledConsumer>(ctx);
                    });

                    cfg.ReceiveEndpoint(QueueNames.FlightCancelled, endpoint =>
                    {
                        endpoint.PrefetchCount = 16;
                        endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(10)));
                        endpoint.ConfigureConsumer<FlightCancelledConsumer>(ctx);
                    });

                    cfg.ReceiveEndpoint(QueueNames.FlightReminder, endpoint =>
                    {
                        endpoint.PrefetchCount = 16;
                        endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                        endpoint.ConfigureConsumer<FlightReminderConsumer>(ctx);
                    });

                    cfg.ReceiveEndpoint(QueueNames.VerificationCode, endpoint =>
                    {
                        endpoint.PrefetchCount = 16;
                        endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                        endpoint.ConfigureConsumer<VerificationCodeConsumer>(ctx);
                    });
                });
            });

            return services;
        }
    }
}
