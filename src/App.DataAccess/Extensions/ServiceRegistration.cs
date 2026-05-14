using App.DataAccess.Context;

namespace App.DataAccess.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionOptions>(configuration.GetSection(ConnectionOptions.Connections));
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.EmailConfiguration));
            services.Configure<App.Core.Options.TokenOptions>(configuration.GetSection(App.Core.Options.TokenOptions.TokenConfiguration));
            services.Configure<TwilioOptions>(configuration.GetSection(TwilioOptions.TwilioConfiguration));

            var connectionOptions = configuration.GetSection(ConnectionOptions.Connections).Get<ConnectionOptions>();

            services.AddDbContext<FlightReservationDbContext>(options =>
            {
                options.UseLazyLoadingProxies();

                if (connectionOptions?.DatabaseProvider == "PostgreSql")
                {
                    options.UseNpgsql(connectionOptions.PostgreSql,
                        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(10), null)).UseSnakeCaseNamingConvention();
                }
                else
                {
                    options.UseSqlServer(connectionOptions?.MssqlServer,
                        sqlOptions => sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(10), null));
                }
            });

            services.AddIdentity<IdentityUser, IdentityRole>(opts =>
            {
                opts.Password.RequiredLength = 8;
                opts.Password.RequiredUniqueChars = 4;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireDigit = true;
                opts.Password.RequireNonAlphanumeric = true;
                opts.SignIn.RequireConfirmedEmail = false;
                opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opts.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<FlightReservationDbContext>()
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddDefaultTokenProviders();

            return services;
        }
    }

    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length) =>
            new() { Code = nameof(PasswordTooShort), Description = $"Password must be at least {length} characters." };

        public override IdentityError PasswordRequiresNonAlphanumeric() =>
            new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Password must contain at least one special character." };

        public override IdentityError PasswordRequiresDigit() =>
            new() { Code = nameof(PasswordRequiresDigit), Description = "Password must contain at least one digit." };

        public override IdentityError PasswordRequiresLower() =>
            new() { Code = nameof(PasswordRequiresLower), Description = "Password must contain at least one lowercase letter." };

        public override IdentityError PasswordRequiresUpper() =>
            new() { Code = nameof(PasswordRequiresUpper), Description = "Password must contain at least one uppercase letter." };

        public override IdentityError DuplicateEmail(string email) =>
            new() { Code = nameof(DuplicateEmail), Description = $"Email '{email}' is already taken." };

        public override IdentityError DuplicateUserName(string userName) =>
            new() { Code = nameof(DuplicateUserName), Description = $"Username '{userName}' is already taken." };
    }
}

