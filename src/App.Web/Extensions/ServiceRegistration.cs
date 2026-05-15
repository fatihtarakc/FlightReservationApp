using App.Web.Services;
using App.Web.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
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
                .AddDebug();
            return builder;
        }

        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001/";
            services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

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
