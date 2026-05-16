using App.Web.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace App.Web.Extensions
{
    public static class ServiceRegistration
    {
        public static WebApplicationBuilder AddLoggingConfiguration(this WebApplicationBuilder builder)
        {
            builder.Logging
                .AddConfiguration(builder.Configuration.GetSection("Logging"))
                .ClearProviders()
                .AddConsole()
                .AddDebug()
                .AddSeq(builder.Configuration.GetSection("Seq"));
            return builder;
        }

        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5000/";
            services.AddTransient<CultureDelegatingHandler>();
            services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(30);
            }).AddHttpMessageHandler<CultureDelegatingHandler>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<ISeatService, SeatService>();
            services.AddScoped<IAircraftService, AircraftService>();
            services.AddScoped<IAirportService, AirportService>();
            services.AddScoped<IAdminService, AdminService>();

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Program>();
            services.AddHttpContextAccessor();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/SignIn";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                    options.SlidingExpiration = true;
                });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "FlightApp.Session";
            });

            return services;
        }
    }
}
