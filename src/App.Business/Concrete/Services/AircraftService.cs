namespace App.Business.Concrete.Services
{
    public class AircraftService : IAircraftService
    {
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService<Aircraft> _cacheService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<AircraftService> _logger;

        private const string CacheKeyAll = "Aircrafts:All";
        private static string CacheKeyById(Guid id) => $"Aircraft:{id}";

        public AircraftService(
            IAircraftRepository aircraftRepository,
            IUnitOfWork unitOfWork,
            ICacheService<Aircraft> cacheService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<AircraftService> logger)
        {
            _aircraftRepository = aircraftRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<AircraftDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var cached = await _cacheService.GetByAsync(CacheKeyById(id));
                if (cached.IsSuccess && cached.Data != null)
                    return new SuccessDataResult<AircraftDto>(cached.Data.Adapt<AircraftDto>(), _localizer[Messages.Aircraft_Was_Found]);

                var aircraft = await _aircraftRepository.IncludeGetByIdAsync(id, tracking: false);
                if (aircraft == null)
                    return new ErrorDataResult<AircraftDto>(_localizer[Messages.Aircraft_Was_Not_Found]);

                await _cacheService.AddAsync(CacheKeyById(id), aircraft);
                return new SuccessDataResult<AircraftDto>(aircraft.Adapt<AircraftDto>(), _localizer[Messages.Aircraft_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<AircraftDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<AircraftListDto>>> GetAllAsync()
        {
            try
            {
                var cachedList = await _cacheService.GetListByAsync(CacheKeyAll);
                if (cachedList.IsSuccess && cachedList.Data != null)
                    return new SuccessDataResult<IEnumerable<AircraftListDto>>(cachedList.Data.Select(a => a.Adapt<AircraftListDto>()));

                var aircrafts = await _aircraftRepository.GetAllAsync(tracking: false);
                var list = aircrafts.ToList();
                await _cacheService.AddListAsync(CacheKeyAll, list);

                return new SuccessDataResult<IEnumerable<AircraftListDto>>(list.Select(a => a.Adapt<AircraftListDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<AircraftListDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<AircraftDto>> AddAsync(AircraftAddDto dto)
        {
            try
            {
                var exists = await _aircraftRepository.AnyAsync(a => a.TailNumber == dto.TailNumber);
                if (exists)
                    return new ErrorDataResult<AircraftDto>(_localizer[Messages.Aircraft_TailNumber_Already_Exists]);

                IDataResult<AircraftDto> result = new ErrorDataResult<AircraftDto>(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var aircraft = new Aircraft
                        {
                            TailNumber      = dto.TailNumber,
                            ManufactureYear = dto.ManufactureYear,
                            AircraftStatus  = AircraftStatus.Active,
                            AirlineId       = dto.AirlineId,
                            ModelId         = dto.ModelId
                        };

                        await _aircraftRepository.AddAsync(aircraft);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyAll);

                        var saved = await _aircraftRepository.IncludeGetByIdAsync(aircraft.Id, tracking: false);
                        _logger.LogInformation("{Message} TailNumber: {Tail}", _localizer[Messages.Aircraft_HasBeen_Added].Value, dto.TailNumber);
                        result = new SuccessDataResult<AircraftDto>(saved.Adapt<AircraftDto>(), _localizer[Messages.Aircraft_HasBeen_Added]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} TailNumber: {Tail}", _localizer[Messages.UnexpectedError].Value, dto.TailNumber);
                        result = new ErrorDataResult<AircraftDto>(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<AircraftDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> UpdateAsync(Guid id, AircraftUpdateDto dto)
        {
            try
            {
                var aircraft = await _aircraftRepository.GetByIdAsync(id);
                if (aircraft == null)
                    return new ErrorResult(_localizer[Messages.Aircraft_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        aircraft.TailNumber      = dto.TailNumber;
                        aircraft.ManufactureYear = dto.ManufactureYear;
                        aircraft.AircraftStatus  = dto.AircraftStatus;
                        aircraft.AirlineId       = dto.AirlineId;
                        aircraft.ModelId         = dto.ModelId;

                        await _aircraftRepository.UpdateAsync(aircraft);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} AircraftId: {Id}", _localizer[Messages.Aircraft_Was_Updated].Value, id);
                        result = new SuccessResult(_localizer[Messages.Aircraft_Was_Updated]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} AircraftId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
                var aircraft = await _aircraftRepository.GetByIdAsync(id);
                if (aircraft == null)
                    return new ErrorResult(_localizer[Messages.Aircraft_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        await _aircraftRepository.DeleteAsync(aircraft);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} AircraftId: {Id}", _localizer[Messages.Aircraft_Was_Deleted].Value, id);
                        result = new SuccessResult(_localizer[Messages.Aircraft_Was_Deleted]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} AircraftId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
