namespace App.Business.Concrete.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IAppUserRepository appUserRepository,
            IFlightRepository flightRepository,
            IBookingRepository bookingRepository,
            IStringLocalizer<MessageResources> localizer,
            ILogger<AdminService> logger)
        {
            _appUserRepository = appUserRepository;
            _flightRepository = flightRepository;
            _bookingRepository = bookingRepository;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<AdminDashboardDto>> GetDashboardAsync()
        {
            try
            {
                var users = await _appUserRepository.GetAllAsync(tracking: false);
                var flights = await _flightRepository.GetAllAsync(tracking: false);
                var bookings = await _bookingRepository.GetAllAsync(tracking: false);

                var flightList = flights.ToList();
                var bookingList = bookings.ToList();
                var now = DateTime.UtcNow;

                var dashboard = new AdminDashboardDto
                {
                    TotalUsers              = users.Count(),
                    TotalFlights            = flightList.Count,
                    ScheduledFlights        = flightList.Count(f => f.FlightStatus == FlightStatus.Scheduled),
                    BoardingFlights         = flightList.Count(f => f.FlightStatus == FlightStatus.Boarding),
                    DepartedFlights         = flightList.Count(f => f.FlightStatus == FlightStatus.Departed),
                    ArrivedFlights          = flightList.Count(f => f.FlightStatus == FlightStatus.Arrived),
                    DelayedFlights          = flightList.Count(f => f.FlightStatus == FlightStatus.Delayed),
                    CancelledFlights        = flightList.Count(f => f.FlightStatus == FlightStatus.Cancelled),
                    TotalBookings           = bookingList.Count,
                    ConfirmedBookings       = bookingList.Count(b => b.BookingStatus == BookingStatus.Confirmed),
                    CancelledBookings       = bookingList.Count(b => b.BookingStatus == BookingStatus.Cancelled),
                    CheckedInBookings       = bookingList.Count(b => b.BookingStatus == BookingStatus.CheckedIn),
                    TotalRevenue            = bookingList
                        .Where(b => b.BookingStatus == BookingStatus.Confirmed || b.BookingStatus == BookingStatus.CheckedIn)
                        .Sum(b => b.TotalPrice),
                    UpcomingFlightsNext24Hours = flightList.Count(f =>
                        f.FlightStatus == FlightStatus.Scheduled &&
                        f.DepartureDateTime >= now &&
                        f.DepartureDateTime <= now.AddHours(24))
                };

                return new SuccessDataResult<AdminDashboardDto>(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<AdminDashboardDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<FlightPassengerStatDto>>> GetFlightPassengerStatsAsync()
        {
            try
            {
                var flights = await _flightRepository.GetAllWithStatsAsync(tracking: false);

                var stats = flights.Select(f =>
                {
                    var confirmedPassengers = f.Bookings.Count(b => b.BookingStatus != BookingStatus.Cancelled);
                    var totalCapacity = f.Aircraft?.Model?.MaxPassengerCapacity ?? 0;

                    return new FlightPassengerStatDto
                    {
                        FlightId          = f.Id,
                        FlightNumber      = f.Number ?? string.Empty,
                        DepartureDateTime = f.DepartureDateTime,
                        DepartureCity     = f.Schedule?.Route?.DepartureAirport?.City ?? string.Empty,
                        ArrivalCity       = f.Schedule?.Route?.ArrivalAirport?.City ?? string.Empty,
                        FlightStatus      = f.FlightStatus.ToString(),
                        PassengerCount    = confirmedPassengers,
                        TotalCapacity     = totalCapacity,
                        OccupancyRate     = totalCapacity > 0
                            ? Math.Round((decimal)confirmedPassengers / totalCapacity * 100, 2)
                            : 0
                    };
                });

                return new SuccessDataResult<IEnumerable<FlightPassengerStatDto>>(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<FlightPassengerStatDto>>(_localizer[Messages.UnexpectedError]);
            }
        }
    }
}
