namespace App.BackgroundJobs.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddBackgroundJobServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionOptions = configuration.GetSection(ConnectionOptions.Connections).Get<ConnectionOptions>()
                ?? new ConnectionOptions();

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings();

                if (connectionOptions.DatabaseProvider == "PostgreSql")
                {
                    config.UsePostgreSqlStorage(c =>
                        c.UseNpgsqlConnection(connectionOptions.Hangfire));
                }
                else
                {
                    config.UseSqlServerStorage(connectionOptions.Hangfire, new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    });
                }
            });

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Environment.ProcessorCount * 2;
                options.Queues = new[] { "default", "critical" };
                options.ServerName = "FlightReservation-HangfireServer";
            });

            services.AddScoped<FlightReminderJob>();

            return services;
        }

        public static void ConfigureRecurringJobs(this IServiceProvider serviceProvider)
        {
            var manager = serviceProvider.GetRequiredService<IRecurringJobManager>();

            manager.AddOrUpdate<FlightReminderJob>(
                RecurringJobSchedules.FlightReminder7Days,
                job => job.Send7DayRemindersAsync(),
                RecurringJobSchedules.DailyAt8AM,
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

            manager.AddOrUpdate<FlightReminderJob>(
                RecurringJobSchedules.FlightReminder24Hours,
                job => job.Send24HourRemindersAsync(),
                RecurringJobSchedules.Hourly,
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
        }
    }
}
