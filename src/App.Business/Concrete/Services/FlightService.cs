namespace App.Business.Concrete.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<FlightService> _logger;

        public FlightService(
            IFlightRepository flightRepository,
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint,
            IStringLocalizer<MessageResources> localizer,
            ILogger<FlightService> logger)
        {
            _flightRepository = flightRepository;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<FlightDto>> GetByIdAsync(Guid id)
        {
            var flight = await _flightRepository.IncludeGetByIdAsync(id, tracking: false);
            if (flight == null)
                return new ErrorDataResult<FlightDto>(_localizer[Messages.Flight_Was_Not_Found]);

            return new SuccessDataResult<FlightDto>(flight.Adapt<FlightDto>(), _localizer[Messages.Flight_Was_Found]);
        }

        public async Task<IDataResult<IEnumerable<FlightListDto>>> GetAllAsync()
        {
            var flights = await _flightRepository.GetAllAsync(tracking: false);
            return new SuccessDataResult<IEnumerable<FlightListDto>>(flights.Select(x => x.Adapt<FlightListDto>()));
        }

        public async Task<IDataResult<IEnumerable<FlightListDto>>> SearchAsync(FlightSearchDto dto)
        {
            var flights = await _flightRepository.SearchFlightsAsync(
                dto.DepartureIata, dto.ArrivalIata, dto.DepartureDate);

            return new SuccessDataResult<IEnumerable<FlightListDto>>(flights.Select(x => x.Adapt<FlightListDto>()));
        }

        public async Task<IDataResult<FlightDto>> AddAsync(FlightAddDto dto)
        {
            var exists = await _flightRepository.AnyAsync(f => f.Number == dto.Number && f.DepartureDateTime.Date == dto.DepartureDateTime.Date);
            if (exists)
                return new ErrorDataResult<FlightDto>(_localizer[Messages.Flight_Number_Already_Exists]);

            var flight = new Flight
            {
                Number = dto.Number,
                DepartureDateTime = dto.DepartureDateTime,
                ArrivalDateTime = dto.ArrivalDateTime,
                Duration = dto.ArrivalDateTime - dto.DepartureDateTime,
                BaseEconomyPrice = dto.BaseEconomyPrice,
                BasePremiumEconomyPrice = dto.BasePremiumEconomyPrice,
                BaseBusinessPrice = dto.BaseBusinessPrice,
                BaseFirstClassPrice = dto.BaseFirstClassPrice,
                Currency = dto.Currency,
                FlightStatus = FlightStatus.Scheduled,
                Gate = dto.Gate,
                Terminal = dto.Terminal,
                AircraftId = dto.AircraftId,
                AirlineId = dto.AirlineId,
                ScheduleId = dto.ScheduleId
            };

            await _flightRepository.AddAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            var saved = await _flightRepository.IncludeGetByIdAsync(flight.Id, tracking: false);
            _logger.LogInformation("{Message} Number: {Number}", _localizer[Messages.Flight_HasBeen_Added].Value, dto.Number);
            return new SuccessDataResult<FlightDto>(saved.Adapt<FlightDto>(), _localizer[Messages.Flight_HasBeen_Added]);
        }

        public async Task<IResult> UpdateAsync(Guid id, FlightUpdateDto dto)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
                return new ErrorResult(_localizer[Messages.Flight_Was_Not_Found]);

            flight.BaseEconomyPrice = dto.BaseEconomyPrice;
            flight.BasePremiumEconomyPrice = dto.BasePremiumEconomyPrice;
            flight.BaseBusinessPrice = dto.BaseBusinessPrice;
            flight.BaseFirstClassPrice = dto.BaseFirstClassPrice;
            flight.FlightStatus = dto.FlightStatus;
            flight.Gate = dto.Gate;
            flight.Terminal = dto.Terminal;
            if (!string.IsNullOrWhiteSpace(dto.CancellationReason))
                flight.CancellationReason = dto.CancellationReason;

            await _flightRepository.UpdateAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{Message} FlightId: {Id}", _localizer[Messages.Flight_Was_Updated].Value, id);
            return new SuccessResult(_localizer[Messages.Flight_Was_Updated]);
        }

        public async Task<IResult> CancelAsync(Guid id, string? reason)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
                return new ErrorResult(_localizer[Messages.Flight_Was_Not_Found]);

            flight.FlightStatus = FlightStatus.Cancelled;
            flight.CancellationReason = reason;

            await _flightRepository.UpdateAsync(flight);

            var activeBookings = await _bookingRepository.GetActiveBookingsByFlightIdAsync(id);
            var affectedPassengers = activeBookings.Select(b => new AffectedPassenger
            {
                PnrNumber = b.PnrNumber,
                Name = $"{b.AppUser.Name} {b.AppUser.Surname}",
                Email = b.AppUser.Email!,
                PhoneNumber = b.AppUser.PhoneNumber!,
                PreferredChannel = b.AppUser.PreferredNotificationChannel
            }).ToList();

            await _unitOfWork.SaveChangesAsync();

            if (affectedPassengers.Any())
            {
                var flightFull = await _flightRepository.IncludeGetByIdAsync(id, tracking: false);
                await _publishEndpoint.Publish(new FlightCancelledEvent
                {
                    FlightId = id,
                    FlightNumber = flight.Number,
                    DepartureDateTime = flight.DepartureDateTime,
                    DepartureCity = flightFull?.Schedule?.Route?.DepartureAirport?.City ?? string.Empty,
                    ArrivalCity = flightFull?.Schedule?.Route?.ArrivalAirport?.City ?? string.Empty,
                    CancellationReason = reason,
                    AffectedPassengers = affectedPassengers
                });
            }

            _logger.LogInformation("{Message} FlightId: {Id}", _localizer[Messages.Flight_Was_Cancelled].Value, id);
            return new SuccessResult(_localizer[Messages.Flight_Was_Cancelled]);
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
                return new ErrorResult(_localizer[Messages.Flight_Was_Not_Found]);

            await _flightRepository.DeleteAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult(_localizer[Messages.Flight_Was_Deleted]);
        }
    }
}




