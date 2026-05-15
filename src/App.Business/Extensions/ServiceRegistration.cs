namespace App.Business.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            ConfigureMapster();

            services.AddCacheServices(configuration);

            services.Configure<Core.Options.TokenOptions>(configuration.GetSection(Core.Options.TokenOptions.TokenConfiguration));

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<IVerificationCodeService, VerificationCodeService>();
            services.AddScoped<IAirlineService, AirlineService>();
            services.AddScoped<IAirportService, AirportService>();
            services.AddScoped<IAircraftService, AircraftService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IModelService, ModelService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<ISeatService, SeatService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IAdminService, AdminService>();

            return services;
        }

        private static void ConfigureMapster()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

            config.NewConfig<Aircraft, AircraftDto>()
                .Map(d => d.AirlineName, s => s.Airline != null ? s.Airline.Name : string.Empty)
                .Map(d => d.ModelName, s => s.Model != null ? s.Model.Name : string.Empty);

            config.NewConfig<Aircraft, AircraftListDto>()
                .Map(d => d.AirlineName, s => s.Airline != null ? s.Airline.Name : string.Empty)
                .Map(d => d.ModelName, s => s.Model != null ? s.Model.Name : string.Empty);

            config.NewConfig<Model, ModelDto>()
                .Map(d => d.ManufacturerName, s => s.Manufacturer != null ? s.Manufacturer.Name : string.Empty);

            config.NewConfig<Route, RouteDto>()
                .Map(d => d.DepartureAirportIata, s => s.DepartureAirport != null ? s.DepartureAirport.IataCode : string.Empty)
                .Map(d => d.DepartureCity, s => s.DepartureAirport != null ? s.DepartureAirport.City : string.Empty)
                .Map(d => d.ArrivalAirportIata, s => s.ArrivalAirport != null ? s.ArrivalAirport.IataCode : string.Empty)
                .Map(d => d.ArrivalCity, s => s.ArrivalAirport != null ? s.ArrivalAirport.City : string.Empty);

            config.NewConfig<Schedule, ScheduleDto>()
                .Map(d => d.DepartureAirport, s => s.Route != null && s.Route.DepartureAirport != null
                    ? s.Route.DepartureAirport.IataCode : string.Empty)
                .Map(d => d.ArrivalAirport, s => s.Route != null && s.Route.ArrivalAirport != null
                    ? s.Route.ArrivalAirport.IataCode : string.Empty);

            config.NewConfig<Flight, FlightDto>()
                .Map(d => d.AirlineName, s => s.Airline != null ? s.Airline.Name : string.Empty)
                .Map(d => d.AirlineIata, s => s.Airline != null ? s.Airline.IataCode : string.Empty)
                .Map(d => d.AircraftTailNumber, s => s.Aircraft != null ? s.Aircraft.TailNumber : string.Empty)
                .Map(d => d.ModelName, s => s.Aircraft != null && s.Aircraft.Model != null ? s.Aircraft.Model.Name : string.Empty)
                .Map(d => d.DepartureAirportIata, s => s.Schedule != null && s.Schedule.Route != null && s.Schedule.Route.DepartureAirport != null
                    ? s.Schedule.Route.DepartureAirport.IataCode : string.Empty)
                .Map(d => d.DepartureCity, s => s.Schedule != null && s.Schedule.Route != null && s.Schedule.Route.DepartureAirport != null
                    ? s.Schedule.Route.DepartureAirport.City : string.Empty)
                .Map(d => d.ArrivalAirportIata, s => s.Schedule != null && s.Schedule.Route != null && s.Schedule.Route.ArrivalAirport != null
                    ? s.Schedule.Route.ArrivalAirport.IataCode : string.Empty)
                .Map(d => d.ArrivalCity, s => s.Schedule != null && s.Schedule.Route != null && s.Schedule.Route.ArrivalAirport != null
                    ? s.Schedule.Route.ArrivalAirport.City : string.Empty)
                .Map(d => d.AvailableEconomySeats, s => 0)
                .Map(d => d.AvailableBusinessSeats, s => 0)
                .Map(d => d.AvailableFirstClassSeats, s => 0);

            config.NewConfig<Flight, FlightListDto>()
                .Map(d => d.AirlineName, s => s.Airline != null ? s.Airline.Name : string.Empty)
                .Map(d => d.DepartureAirportIata, s => s.Schedule != null && s.Schedule.Route != null && s.Schedule.Route.DepartureAirport != null
                    ? s.Schedule.Route.DepartureAirport.IataCode : string.Empty)
                .Map(d => d.DepartureCity, s => s.Schedule != null && s.Schedule.Route != null && s.Schedule.Route.DepartureAirport != null
                    ? s.Schedule.Route.DepartureAirport.City : string.Empty)
                .Map(d => d.ArrivalAirportIata, s => s.Schedule != null && s.Schedule.Route != null && s.Schedule.Route.ArrivalAirport != null
                    ? s.Schedule.Route.ArrivalAirport.IataCode : string.Empty)
                .Map(d => d.ArrivalCity, s => s.Schedule != null && s.Schedule.Route != null && s.Schedule.Route.ArrivalAirport != null
                    ? s.Schedule.Route.ArrivalAirport.City : string.Empty)
                .Map(d => d.AvailableSeats, s => 0);

            config.NewConfig<Seat, SeatDto>()
                .Map(d => d.IsAvailable, s => false);

            config.NewConfig<Booking, BookingDto>()
                .Map(d => d.PassengerName, s => s.AppUser != null
                    ? $"{s.AppUser.Name} {s.AppUser.Surname}" : string.Empty)
                .Map(d => d.FlightNumber, s => s.Flight != null ? s.Flight.Number : string.Empty)
                .Map(d => d.DepartureDateTime, s => s.Flight != null ? s.Flight.DepartureDateTime : default)
                .Map(d => d.DepartureCity, s => s.Flight != null && s.Flight.Schedule != null
                    && s.Flight.Schedule.Route != null && s.Flight.Schedule.Route.DepartureAirport != null
                    ? s.Flight.Schedule.Route.DepartureAirport.City : string.Empty)
                .Map(d => d.ArrivalCity, s => s.Flight != null && s.Flight.Schedule != null
                    && s.Flight.Schedule.Route != null && s.Flight.Schedule.Route.ArrivalAirport != null
                    ? s.Flight.Schedule.Route.ArrivalAirport.City : string.Empty)
                .Map(d => d.SeatNumber, s => s.Seat != null ? $"{s.Seat.Row}{s.Seat.Column}" : string.Empty)
                .Map(d => d.SeatClass, s => s.Seat != null ? s.Seat.SeatClass : SeatClass.Economy);

            config.NewConfig<Booking, BookingListDto>()
                .Map(d => d.FlightNumber, s => s.Flight != null ? s.Flight.Number : string.Empty)
                .Map(d => d.DepartureDateTime, s => s.Flight != null ? s.Flight.DepartureDateTime : default)
                .Map(d => d.DepartureCity, s => s.Flight != null && s.Flight.Schedule != null
                    && s.Flight.Schedule.Route != null && s.Flight.Schedule.Route.DepartureAirport != null
                    ? s.Flight.Schedule.Route.DepartureAirport.City : string.Empty)
                .Map(d => d.ArrivalCity, s => s.Flight != null && s.Flight.Schedule != null
                    && s.Flight.Schedule.Route != null && s.Flight.Schedule.Route.ArrivalAirport != null
                    ? s.Flight.Schedule.Route.ArrivalAirport.City : string.Empty)
                .Map(d => d.SeatNumber, s => s.Seat != null ? $"{s.Seat.Row}{s.Seat.Column}" : string.Empty)
                .Map(d => d.SeatClass, s => s.Seat != null ? s.Seat.SeatClass : SeatClass.Economy);
        }
    }
}
