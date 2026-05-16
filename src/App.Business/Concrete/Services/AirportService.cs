namespace App.Business.Concrete.Services
{
    public class AirportService : IAirportService
    {
        private readonly IAirportRepository _airportRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService<Airport> _cacheService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<AirportService> _logger;

        private const string CacheKeyAll = "Airports:All";
        private static string CacheKeyById(Guid id) => $"Airport:{id}";

        public AirportService(
            IAirportRepository airportRepository,
            IUnitOfWork unitOfWork,
            ICacheService<Airport> cacheService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<AirportService> logger)
        {
            _airportRepository = airportRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<AirportDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var cached = await _cacheService.GetByAsync(CacheKeyById(id));
                if (cached.IsSuccess && cached.Data != null)
                    return new SuccessDataResult<AirportDto>(cached.Data.Adapt<AirportDto>(), _localizer[Messages.Airport_Was_Found]);

                var airport = await _airportRepository.GetByIdAsync(id, tracking: false);
                if (airport == null)
                    return new ErrorDataResult<AirportDto>(_localizer[Messages.Airport_Was_Not_Found]);

                await _cacheService.AddAsync(CacheKeyById(id), airport);
                return new SuccessDataResult<AirportDto>(airport.Adapt<AirportDto>(), _localizer[Messages.Airport_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<AirportDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<AirportListDto>>> GetAllAsync()
        {
            try
            {
                var cachedList = await _cacheService.GetListByAsync(CacheKeyAll);
                if (cachedList.IsSuccess && cachedList.Data != null)
                    return new SuccessDataResult<IEnumerable<AirportListDto>>(cachedList.Data.Select(x => x.Adapt<AirportListDto>()));

                var airports = await _airportRepository.GetAllAsync(tracking: false);
                var airportList = airports.ToList();
                await _cacheService.AddListAsync(CacheKeyAll, airportList);

                return new SuccessDataResult<IEnumerable<AirportListDto>>(airportList.Select(x => x.Adapt<AirportListDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<AirportListDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<AirportDto>> AddAsync(AirportAddDto dto)
        {
            try
            {
                var exists = await _airportRepository.AnyAsync(a => a.IataCode == dto.IataCode);
                if (exists)
                    return new ErrorDataResult<AirportDto>(_localizer[Messages.Airport_IataCode_Already_Exists]);

                IDataResult<AirportDto> result = new ErrorDataResult<AirportDto>(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var airport = new Airport
                        {
                            Name      = dto.Name,
                            IataCode  = dto.IataCode,
                            IcaoCode  = dto.IcaoCode,
                            City      = dto.City,
                            Country   = dto.Country,
                            TimeZone  = dto.TimeZone,
                            Latitude  = dto.Latitude,
                            Longitude = dto.Longitude
                        };

                        await _airportRepository.AddAsync(airport);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} IataCode: {Code}", _localizer[Messages.Airport_HasBeen_Added].Value, dto.IataCode);
                        result = new SuccessDataResult<AirportDto>(airport.Adapt<AirportDto>(), _localizer[Messages.Airport_HasBeen_Added]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} IataCode: {Code}", _localizer[Messages.UnexpectedError].Value, dto.IataCode);
                        result = new ErrorDataResult<AirportDto>(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<AirportDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> UpdateAsync(Guid id, AirportUpdateDto dto)
        {
            try
            {
                var airport = await _airportRepository.GetByIdAsync(id);
                if (airport == null)
                    return new ErrorResult(_localizer[Messages.Airport_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        airport.Name      = dto.Name;
                        airport.IataCode  = dto.IataCode;
                        airport.IcaoCode  = dto.IcaoCode;
                        airport.City      = dto.City;
                        airport.Country   = dto.Country;
                        airport.TimeZone  = dto.TimeZone;
                        airport.Latitude  = dto.Latitude;
                        airport.Longitude = dto.Longitude;

                        await _airportRepository.UpdateAsync(airport);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} AirportId: {Id}", _localizer[Messages.Airport_Was_Updated].Value, id);
                        result = new SuccessResult(_localizer[Messages.Airport_Was_Updated]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} AirportId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
                var airport = await _airportRepository.GetByIdAsync(id);
                if (airport == null)
                    return new ErrorResult(_localizer[Messages.Airport_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        await _airportRepository.DeleteAsync(airport);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} AirportId: {Id}", _localizer[Messages.Airport_Was_Deleted].Value, id);
                        result = new SuccessResult(_localizer[Messages.Airport_Was_Deleted]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} AirportId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
