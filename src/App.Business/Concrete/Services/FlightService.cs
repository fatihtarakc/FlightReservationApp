namespace App.Business.Concrete.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICacheService<Flight> _cacheService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<FlightService> _logger;

        private const string CacheKeyAll = "Flights:All";
        private static string CacheKeyById(Guid id) => $"Flight:{id}";

        public FlightService(
            IFlightRepository flightRepository,
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint,
            ICacheService<Flight> cacheService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<FlightService> logger)
        {
            _flightRepository = flightRepository;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
            _cacheService = cacheService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<FlightDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var cached = await _cacheService.GetByAsync(CacheKeyById(id));
                if (cached.IsSuccess && cached.Data is not null) return new SuccessDataResult<FlightDto>(cached.Data.Adapt<FlightDto>(), _localizer[Messages.Flight_Was_Found]);

                var flight = await _flightRepository.IncludeGetByIdAsync(id, tracking: false);
                if (flight is null) return new ErrorDataResult<FlightDto>(_localizer[Messages.Flight_Was_Not_Found]);

                await _cacheService.AddAsync(CacheKeyById(id), flight);
                return new SuccessDataResult<FlightDto>(flight.Adapt<FlightDto>(), _localizer[Messages.Flight_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<FlightDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<FlightListDto>>> GetAllAsync()
        {
            try
            {
                var cached = await _cacheService.GetListByAsync(CacheKeyAll);
                if (cached.IsSuccess && cached.Data is not null) return new SuccessDataResult<IEnumerable<FlightListDto>>(cached.Data.Select(x => x.Adapt<FlightListDto>()));

                var flights = await _flightRepository.GetAllAsync(tracking: false);
                var list = flights.Select(f => f.Adapt<FlightListDto>()).ToList();
                await _cacheService.AddListAsync(CacheKeyAll, flights);

                return new SuccessDataResult<IEnumerable<FlightListDto>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<FlightListDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<FlightListDto>>> SearchAsync(FlightSearchDto dto)
        {
            try
            {
                var flights = await _flightRepository.SearchFlightsAsync(
                    dto.DepartureIata, dto.ArrivalIata, dto.DepartureDate);

                return new SuccessDataResult<IEnumerable<FlightListDto>>(flights.Select(x => x.Adapt<FlightListDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<FlightListDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<FlightDto>> AddAsync(FlightAddDto dto)
        {
            try
            {
                var exists = await _flightRepository.AnyAsync(f =>
                    f.Number == dto.Number && f.DepartureDateTime.Date == dto.DepartureDateTime.Date);
                if (exists)
                    return new ErrorDataResult<FlightDto>(_localizer[Messages.Flight_Number_Already_Exists]);

                IDataResult<FlightDto> result = new ErrorDataResult<FlightDto>(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var flight = new Flight
                        {
                            Number                  = dto.Number,
                            DepartureDateTime       = dto.DepartureDateTime,
                            ArrivalDateTime         = dto.ArrivalDateTime,
                            Duration                = dto.ArrivalDateTime - dto.DepartureDateTime,
                            BaseEconomyPrice        = dto.BaseEconomyPrice,
                            BasePremiumEconomyPrice = dto.BasePremiumEconomyPrice,
                            BaseBusinessPrice       = dto.BaseBusinessPrice,
                            BaseFirstClassPrice     = dto.BaseFirstClassPrice,
                            Currency                = dto.Currency,
                            FlightStatus            = FlightStatus.Scheduled,
                            Gate                    = dto.Gate,
                            Terminal                = dto.Terminal,
                            AircraftId              = dto.AircraftId,
                            AirlineId               = dto.AirlineId,
                            ScheduleId              = dto.ScheduleId
                        };

                        await _flightRepository.AddAsync(flight);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyAll);

                        var saved = await _flightRepository.IncludeGetByIdAsync(flight.Id, tracking: false);
                        _logger.LogInformation("{Message} Number: {Number}", _localizer[Messages.Flight_HasBeen_Added].Value, dto.Number);
                        result = new SuccessDataResult<FlightDto>(saved.Adapt<FlightDto>(), _localizer[Messages.Flight_HasBeen_Added]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} Number: {Number}", _localizer[Messages.UnexpectedError].Value, dto.Number);
                        result = new ErrorDataResult<FlightDto>(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<FlightDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> UpdateAsync(Guid id, FlightUpdateDto dto)
        {
            try
            {
                var flight = await _flightRepository.GetByIdAsync(id);
                if (flight == null)
                    return new ErrorResult(_localizer[Messages.Flight_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        flight.BaseEconomyPrice        = dto.BaseEconomyPrice;
                        flight.BasePremiumEconomyPrice = dto.BasePremiumEconomyPrice;
                        flight.BaseBusinessPrice       = dto.BaseBusinessPrice;
                        flight.BaseFirstClassPrice     = dto.BaseFirstClassPrice;
                        flight.FlightStatus            = dto.FlightStatus;
                        flight.Gate                    = dto.Gate;
                        flight.Terminal                = dto.Terminal;
                        if (!string.IsNullOrWhiteSpace(dto.CancellationReason))
                            flight.CancellationReason = dto.CancellationReason;

                        await _flightRepository.UpdateAsync(flight);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} FlightId: {Id}", _localizer[Messages.Flight_Was_Updated].Value, id);
                        result = new SuccessResult(_localizer[Messages.Flight_Was_Updated]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} FlightId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
                        result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> CancelAsync(Guid id, string? reason)
        {
            try
            {
                var flight = await _flightRepository.GetByIdAsync(id);
                if (flight == null)
                    return new ErrorResult(_localizer[Messages.Flight_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        flight.FlightStatus       = FlightStatus.Cancelled;
                        flight.CancellationReason = reason;

                        await _flightRepository.UpdateAsync(flight);

                        var activeBookings = await _bookingRepository.GetActiveBookingsByFlightIdAsync(id);
                        var affectedPassengers = activeBookings.Select(b => new AffectedPassenger
                        {
                            PnrNumber        = b.PnrNumber,
                            Name             = $"{b.AppUser!.Name} {b.AppUser!.Surname}",
                            Email            = b.AppUser.Email!,
                            PhoneNumber      = b.AppUser.PhoneNumber!,
                            PreferredChannel = b.AppUser.PreferredNotificationChannel
                        }).ToList();

                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        if (affectedPassengers.Any())
                        {
                            var flightFull = await _flightRepository.IncludeGetByIdAsync(id, tracking: false);
                            await _publishEndpoint.Publish(new FlightCancelledEvent
                            {
                                FlightId           = id,
                                FlightNumber       = flight.Number,
                                DepartureDateTime  = flight.DepartureDateTime,
                                DepartureCity      = flightFull?.Schedule?.Route?.DepartureAirport?.City ?? string.Empty,
                                ArrivalCity        = flightFull?.Schedule?.Route?.ArrivalAirport?.City ?? string.Empty,
                                CancellationReason = reason,
                                AffectedPassengers = affectedPassengers,
                                Language           = CultureInfo.CurrentUICulture.Name
                            });
                        }

                        _logger.LogInformation("{Message} FlightId: {Id}", _localizer[Messages.Flight_Was_Cancelled].Value, id);
                        result = new SuccessResult(_localizer[Messages.Flight_Was_Cancelled]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} FlightId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
                        result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            try
            {
                var flight = await _flightRepository.GetByIdAsync(id);
                if (flight == null)
                    return new ErrorResult(_localizer[Messages.Flight_Was_Not_Found]);

                var activeBookings = await _bookingRepository.GetActiveBookingsByFlightIdAsync(id, tracking: false);
                if (activeBookings.Any())
                    return new ErrorResult(_localizer[Messages.Flight_Has_Active_Bookings]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        await _flightRepository.DeleteAsync(flight);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} FlightId: {Id}", _localizer[Messages.Flight_Was_Deleted].Value, id);
                        result = new SuccessResult(_localizer[Messages.Flight_Was_Deleted]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} FlightId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
                        result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }
    }
}
