namespace App.Business.Concrete.Services
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService<Route> _cacheService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<RouteService> _logger;

        private const string CacheKeyAll = "Routes:All";
        private static string CacheKeyById(Guid id) => $"Route:{id}";

        public RouteService(
            IRouteRepository routeRepository,
            IAirportRepository airportRepository,
            IUnitOfWork unitOfWork,
            ICacheService<Route> cacheService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<RouteService> logger)
        {
            _routeRepository = routeRepository;
            _airportRepository = airportRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<RouteDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var cached = await _cacheService.GetByAsync(CacheKeyById(id));
                if (cached.IsSuccess && cached.Data != null)
                    return new SuccessDataResult<RouteDto>(cached.Data.Adapt<RouteDto>(), _localizer[Messages.Route_Was_Found]);

                var route = await _routeRepository.IncludeGetByIdAsync(id, tracking: false);
                if (route == null)
                    return new ErrorDataResult<RouteDto>(_localizer[Messages.Route_Was_Not_Found]);

                await _cacheService.AddAsync(CacheKeyById(id), route);
                return new SuccessDataResult<RouteDto>(route.Adapt<RouteDto>(), _localizer[Messages.Route_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<RouteDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<RouteDto>>> GetAllAsync()
        {
            try
            {
                var cachedList = await _cacheService.GetListByAsync(CacheKeyAll);
                if (cachedList.IsSuccess && cachedList.Data != null)
                    return new SuccessDataResult<IEnumerable<RouteDto>>(cachedList.Data.Select(x => x.Adapt<RouteDto>()));

                var routes = await _routeRepository.GetAllAsync(tracking: false);
                var list = routes.ToList();
                await _cacheService.AddListAsync(CacheKeyAll, list);

                return new SuccessDataResult<IEnumerable<RouteDto>>(list.Select(x => x.Adapt<RouteDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<RouteDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<RouteDto>> AddAsync(RouteAddDto dto)
        {
            try
            {
                var exists = await _routeRepository.AnyAsync(r =>
                    r.DepartureAirportId == dto.DepartureAirportId &&
                    r.ArrivalAirportId == dto.ArrivalAirportId);
                if (exists)
                    return new ErrorDataResult<RouteDto>(_localizer[Messages.Route_Already_Exists]);

                IDataResult<RouteDto> result = new ErrorDataResult<RouteDto>(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var route = new Route
                        {
                            DepartureAirportId = dto.DepartureAirportId,
                            ArrivalAirportId   = dto.ArrivalAirportId,
                            DistanceKm         = dto.DistanceKm,
                            EstimatedDuration  = dto.EstimatedDuration
                        };

                        await _routeRepository.AddAsync(route);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyAll);

                        var saved = await _routeRepository.IncludeGetByIdAsync(route.Id, tracking: false);
                        _logger.LogInformation("{Message} RouteId: {Id}", _localizer[Messages.Route_HasBeen_Added].Value, route.Id);
                        result = new SuccessDataResult<RouteDto>(saved.Adapt<RouteDto>(), _localizer[Messages.Route_HasBeen_Added]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message}", _localizer[Messages.UnexpectedError].Value);
                        result = new ErrorDataResult<RouteDto>(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<RouteDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> UpdateAsync(Guid id, RouteUpdateDto dto)
        {
            try
            {
                var route = await _routeRepository.GetByIdAsync(id);
                if (route == null)
                    return new ErrorResult(_localizer[Messages.Route_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        route.DistanceKm        = dto.DistanceKm;
                        route.EstimatedDuration = dto.EstimatedDuration;

                        await _routeRepository.UpdateAsync(route);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} RouteId: {Id}", _localizer[Messages.Route_Was_Updated].Value, id);
                        result = new SuccessResult(_localizer[Messages.Route_Was_Updated]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} RouteId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
                var route = await _routeRepository.GetByIdAsync(id);
                if (route == null)
                    return new ErrorResult(_localizer[Messages.Route_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        await _routeRepository.DeleteAsync(route);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} RouteId: {Id}", _localizer[Messages.Route_Was_Deleted].Value, id);
                        result = new SuccessResult(_localizer[Messages.Route_Was_Deleted]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} RouteId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
