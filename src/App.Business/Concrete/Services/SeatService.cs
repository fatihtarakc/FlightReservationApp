namespace App.Business.Concrete.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService<Seat> _cacheService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<SeatService> _logger;

        private static string CacheKeyById(Guid id)               => $"Seat:{id}";
        private static string CacheKeyByAircraft(Guid aircraftId) => $"Seats:Aircraft:{aircraftId}";

        public SeatService(
            ISeatRepository seatRepository,
            IUnitOfWork unitOfWork,
            ICacheService<Seat> cacheService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<SeatService> logger)
        {
            _seatRepository = seatRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<SeatDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var cached = await _cacheService.GetByAsync(CacheKeyById(id));
                if (cached.IsSuccess && cached.Data != null)
                    return new SuccessDataResult<SeatDto>(cached.Data.Adapt<SeatDto>(), _localizer[Messages.Seat_Was_Found]);

                var seat = await _seatRepository.GetByIdAsync(id, tracking: false);
                if (seat == null)
                    return new ErrorDataResult<SeatDto>(_localizer[Messages.Seat_Was_Not_Found]);

                await _cacheService.AddAsync(CacheKeyById(id), seat);
                return new SuccessDataResult<SeatDto>(seat.Adapt<SeatDto>(), _localizer[Messages.Seat_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<SeatDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<SeatDto>>> GetByAircraftIdAsync(Guid aircraftId)
        {
            try
            {
                var cachedList = await _cacheService.GetListByAsync(CacheKeyByAircraft(aircraftId));
                if (cachedList.IsSuccess && cachedList.Data != null)
                    return new SuccessDataResult<IEnumerable<SeatDto>>(cachedList.Data.Select(s => s.Adapt<SeatDto>()));

                var seats = await _seatRepository.GetByAircraftIdAsync(aircraftId, tracking: false);
                var list = seats.ToList();
                await _cacheService.AddListAsync(CacheKeyByAircraft(aircraftId), list);

                return new SuccessDataResult<IEnumerable<SeatDto>>(list.Select(s => s.Adapt<SeatDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<SeatDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<SeatDto>>> GetAvailableByFlightIdAsync(Guid flightId)
        {
            try
            {
                var seats = await _seatRepository.GetAvailableSeatsByFlightIdAsync(flightId, tracking: false);
                return new SuccessDataResult<IEnumerable<SeatDto>>(seats.Select(s => s.Adapt<SeatDto>() with { IsAvailable = true }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<SeatDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<SeatDto>>> GetAllWithAvailabilityByFlightIdAsync(Guid flightId)
        {
            try
            {
                var allSeats = await _seatRepository.GetAllByFlightAircraftAsync(flightId, tracking: false);
                var available = await _seatRepository.GetAvailableSeatsByFlightIdAsync(flightId, tracking: false);
                var availableIds = available.Select(s => s.Id).ToHashSet();
                var dtos = allSeats.Select(s => s.Adapt<SeatDto>() with { IsAvailable = availableIds.Contains(s.Id) });
                return new SuccessDataResult<IEnumerable<SeatDto>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<SeatDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> AddAsync(SeatAddDto dto)
        {
            try
            {
                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var seat = new Seat
                        {
                            Row              = dto.Row,
                            Column           = dto.Column,
                            SeatClass        = dto.SeatClass,
                            IsWindowSeat     = dto.IsWindowSeat,
                            IsAisleSeat      = dto.IsAisleSeat,
                            HasExtraLegRoom  = dto.HasExtraLegRoom,
                            AircraftId       = dto.AircraftId
                        };

                        await _seatRepository.AddAsync(seat);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyByAircraft(dto.AircraftId));

                        _logger.LogInformation("{Message} Row: {Row} Column: {Col}", _localizer[Messages.Seat_HasBeen_Added].Value, dto.Row, dto.Column);
                        result = new SuccessResult(_localizer[Messages.Seat_HasBeen_Added]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message}", _localizer[Messages.UnexpectedError].Value);
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

        public async Task<IResult> AddRangeAsync(IEnumerable<SeatAddDto> dtos)
        {
            try
            {
                var dtoList = dtos.ToList();
                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var seats = dtoList.Select(dto => new Seat
                        {
                            Row             = dto.Row,
                            Column          = dto.Column,
                            SeatClass       = dto.SeatClass,
                            IsWindowSeat    = dto.IsWindowSeat,
                            IsAisleSeat     = dto.IsAisleSeat,
                            HasExtraLegRoom = dto.HasExtraLegRoom,
                            AircraftId      = dto.AircraftId
                        });

                        await _seatRepository.AddRangeAsync(seats);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        foreach (var aircraftId in dtoList.Select(d => d.AircraftId).Distinct())
                            await _cacheService.DeleteAsync(CacheKeyByAircraft(aircraftId));

                        result = new SuccessResult(_localizer[Messages.Seat_HasBeen_Added]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message}", _localizer[Messages.UnexpectedError].Value);
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
                var seat = await _seatRepository.GetByIdAsync(id);
                if (seat == null)
                    return new ErrorResult(_localizer[Messages.Seat_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var aircraftId = seat.AircraftId;
                        await _seatRepository.DeleteAsync(seat);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyByAircraft(aircraftId));

                        result = new SuccessResult(_localizer[Messages.Seat_Was_Deleted]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} SeatId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
