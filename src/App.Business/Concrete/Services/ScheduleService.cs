namespace App.Business.Concrete.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService<Schedule> _cacheService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<ScheduleService> _logger;

        private const string CacheKeyAll = "Schedules:All";
        private static string CacheKeyById(Guid id) => $"Schedule:{id}";
        private static string CacheKeyByRoute(Guid routeId) => $"Schedules:Route:{routeId}";

        public ScheduleService(
            IScheduleRepository scheduleRepository,
            IRouteRepository routeRepository,
            IUnitOfWork unitOfWork,
            ICacheService<Schedule> cacheService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<ScheduleService> logger)
        {
            _scheduleRepository = scheduleRepository;
            _routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<ScheduleDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var cached = await _cacheService.GetByAsync(CacheKeyById(id));
                if (cached.IsSuccess && cached.Data is not null) return new SuccessDataResult<ScheduleDto>(cached.Data.Adapt<ScheduleDto>(), _localizer[Messages.Schedule_Was_Found]);

                var schedule = await _scheduleRepository.GetByIdAsync(id, tracking: false);
                if (schedule is null) return new ErrorDataResult<ScheduleDto>(_localizer[Messages.Schedule_Was_Not_Found]);

                await _cacheService.AddAsync(CacheKeyById(id), schedule);
                return new SuccessDataResult<ScheduleDto>(schedule.Adapt<ScheduleDto>(), _localizer[Messages.Schedule_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<ScheduleDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<ScheduleDto>>> GetAllAsync()
        {
            try
            {
                var cachedList = await _cacheService.GetListByAsync(CacheKeyAll);
                if (cachedList.IsSuccess && cachedList.Data is not null) return new SuccessDataResult<IEnumerable<ScheduleDto>>(cachedList.Data.Select(x => x.Adapt<ScheduleDto>()));

                var schedules = await _scheduleRepository.GetAllAsync(tracking: false);
                var list = schedules.ToList();
                await _cacheService.AddListAsync(CacheKeyAll, list);
                return new SuccessDataResult<IEnumerable<ScheduleDto>>(list.Select(x => x.Adapt<ScheduleDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<ScheduleDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<ScheduleDto>>> GetByRouteIdAsync(Guid routeId)
        {
            try
            {
                var cachedList = await _cacheService.GetListByAsync(CacheKeyByRoute(routeId));
                if (cachedList.IsSuccess && cachedList.Data != null)
                    return new SuccessDataResult<IEnumerable<ScheduleDto>>(cachedList.Data.Select(x => x.Adapt<ScheduleDto>()));

                var schedules = await _scheduleRepository.GetByRouteIdAsync(routeId, tracking: false);
                var list = schedules.ToList();
                await _cacheService.AddListAsync(CacheKeyByRoute(routeId), list);
                return new SuccessDataResult<IEnumerable<ScheduleDto>>(list.Select(x => x.Adapt<ScheduleDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<ScheduleDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<ScheduleDto>> AddAsync(ScheduleAddDto dto)
        {
            try
            {
                IDataResult<ScheduleDto> result = new ErrorDataResult<ScheduleDto>(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var schedule = new Schedule
                        {
                            Code = dto.Code,
                            ValidFrom = dto.ValidFrom,
                            ValidTo = dto.ValidTo,
                            DaysOfWeek = dto.DaysOfWeek,
                            DepartureTime = dto.DepartureTime,
                            TimeZone = dto.TimeZone,
                            RouteId = dto.RouteId
                        };

                        await _scheduleRepository.AddAsync(schedule);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyAll);
                        await _cacheService.DeleteAsync(CacheKeyByRoute(dto.RouteId));

                        _logger.LogInformation("{Message} Code: {Code}", _localizer[Messages.Schedule_HasBeen_Added].Value, dto.Code);
                        result = new SuccessDataResult<ScheduleDto>(schedule.Adapt<ScheduleDto>(), _localizer[Messages.Schedule_HasBeen_Added]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} Code: {Code}", _localizer[Messages.UnexpectedError].Value, dto.Code);
                        result = new ErrorDataResult<ScheduleDto>(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<ScheduleDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> UpdateAsync(Guid id, ScheduleUpdateDto dto)
        {
            try
            {
                var schedule = await _scheduleRepository.GetByIdAsync(id);
                if (schedule == null)
                    return new ErrorResult(_localizer[Messages.Schedule_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        schedule.ValidFrom = dto.ValidFrom;
                        schedule.ValidTo = dto.ValidTo;
                        schedule.DaysOfWeek = dto.DaysOfWeek;
                        schedule.DepartureTime = dto.DepartureTime;
                        schedule.TimeZone = dto.TimeZone;

                        await _scheduleRepository.UpdateAsync(schedule);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);
                        await _cacheService.DeleteAsync(CacheKeyByRoute(schedule.RouteId));

                        _logger.LogInformation("{Message} ScheduleId: {Id}", _localizer[Messages.Schedule_Was_Updated].Value, id);
                        result = new SuccessResult(_localizer[Messages.Schedule_Was_Updated]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} ScheduleId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
                var schedule = await _scheduleRepository.GetByIdAsync(id);
                if (schedule == null)
                    return new ErrorResult(_localizer[Messages.Schedule_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        await _scheduleRepository.DeleteAsync(schedule);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);
                        await _cacheService.DeleteAsync(CacheKeyByRoute(schedule.RouteId));

                        _logger.LogInformation("{Message} ScheduleId: {Id}", _localizer[Messages.Schedule_Was_Deleted].Value, id);
                        result = new SuccessResult(_localizer[Messages.Schedule_Was_Deleted]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} ScheduleId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
