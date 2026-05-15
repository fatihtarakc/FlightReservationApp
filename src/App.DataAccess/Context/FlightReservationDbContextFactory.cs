namespace App.DataAccess.Context
{
    public class FlightReservationDbContextFactory : IDesignTimeDbContextFactory<FlightReservationDbContext>
    {
        public FlightReservationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "App.Api"))
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionOptions = configuration.GetSection(ConnectionOptions.Connections).Get<ConnectionOptions>();
            var optionsBuilder = new DbContextOptionsBuilder<FlightReservationDbContext>();

            if (connectionOptions?.DatabaseProvider == "PostgreSql")
            {
                optionsBuilder.UseNpgsql(connectionOptions.PostgreSql,
                    o => o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)).UseSnakeCaseNamingConvention();
            }
            else
            {
                optionsBuilder.UseSqlServer(connectionOptions?.MssqlServer,
                    o => o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
            }

            return new FlightReservationDbContext(optionsBuilder.Options);
        }
    }
}

