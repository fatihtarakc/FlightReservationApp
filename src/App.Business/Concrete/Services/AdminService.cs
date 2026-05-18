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
                var users    = (await _appUserRepository.GetAllAsync(tracking: false)).OrderBy(user => user.Name);
                var flights  = (await _flightRepository.GetAllWithStatsAsync(tracking: false)).OrderBy(flight => flight.Number);
                var bookings = (await _bookingRepository.GetAllWithDetailsAsync(tracking: false)).OrderBy(booking => booking.PnrNumber);

                var flightList  = flights.ToList();
                var bookingList = bookings.ToList();
                var now = DateTime.UtcNow;

                var recentFlights = flightList
                    .OrderByDescending(f => f.DepartureDateTime)
                    .Take(5)
                    .Select(f => new FlightListDto(
                        f.Id,
                        f.Number ?? string.Empty,
                        f.DepartureDateTime,
                        f.ArrivalDateTime,
                        f.Airline?.Name ?? string.Empty,
                        f.Schedule?.Route?.DepartureAirport?.IataCode ?? string.Empty,
                        f.Schedule?.Route?.DepartureAirport?.City ?? string.Empty,
                        f.Schedule?.Route?.ArrivalAirport?.IataCode ?? string.Empty,
                        f.Schedule?.Route?.ArrivalAirport?.City ?? string.Empty,
                        f.BaseEconomyPrice,
                        f.Currency,
                        f.FlightStatus,
                        0, 0, 0, 0))
                    .ToList();

                var recentBookings = bookingList
                    .Take(5)
                    .Select(b => new BookingDto(
                        b.Id,
                        b.PnrNumber ?? string.Empty,
                        b.TotalPrice,
                        b.Currency,
                        b.BookingStatus,
                        b.CheckInTime,
                        b.BoardingPassNumber,
                        b.AppUserId,
                        $"{b.AppUser?.Name} {b.AppUser?.Surname}".Trim(),
                        b.FlightId,
                        b.Flight?.Number ?? string.Empty,
                        b.Flight?.DepartureDateTime ?? DateTime.MinValue,
                        b.Flight?.Schedule?.Route?.DepartureAirport?.City ?? string.Empty,
                        b.Flight?.Schedule?.Route?.ArrivalAirport?.City ?? string.Empty,
                        $"{b.Seat?.Row}{b.Seat?.Column}",
                        b.Seat?.SeatClass ?? SeatClass.Economy,
                        b.CreatedDate))
                    .ToList();

                var userList = users.ToList();
                var dashboard = new AdminDashboardDto
                {
                    TotalUsers              = userList.Count,
                    ActiveUsers             = userList.Count(u => u.UserStatus == UserStatus.Active),
                    TotalFlights            = flightList.Count,
                    ActiveFlights           = flightList.Count(f => f.FlightStatus == FlightStatus.Scheduled
                                                                 || f.FlightStatus == FlightStatus.Boarding
                                                                 || f.FlightStatus == FlightStatus.Departed),
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
                        f.DepartureDateTime <= now.AddHours(24)),
                    RecentFlights  = recentFlights,
                    RecentBookings = recentBookings
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
                var flights = (await _flightRepository.GetAllWithStatsAsync(tracking: false)).OrderBy(flight => flight.Number);

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
