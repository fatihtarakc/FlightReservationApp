using App.DataAccess.Concrete.Repositories.Concrete;
using App.DataAccess.Concrete.UnitOfWorks.Concrete;

namespace App.DataAccess.Concrete.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddDataAccessConcreteServices(this IServiceCollection services)
        {
            services.AddScoped<IAircraftRepository, AircraftRepository>();
            services.AddScoped<IAirlineRepository, AirlineRepository>();
            services.AddScoped<IAirportRepository, AirportRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IFlightRepository, FlightRepository>();
            services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            services.AddScoped<IModelRepository, ModelRepository>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<ISeatRepository, SeatRepository>();
            services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
