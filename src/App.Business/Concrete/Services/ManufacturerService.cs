namespace App.Business.Concrete.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService<Manufacturer> _cacheService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<ManufacturerService> _logger;

        private const string CacheKeyAll = "Manufacturers:All";
        private static string CacheKeyById(Guid id) => $"Manufacturer:{id}";

        public ManufacturerService(
            IManufacturerRepository manufacturerRepository,
            IUnitOfWork unitOfWork,
            ICacheService<Manufacturer> cacheService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<ManufacturerService> logger)
        {
            _manufacturerRepository = manufacturerRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<ManufacturerDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var cached = await _cacheService.GetByAsync(CacheKeyById(id));
                if (cached.IsSuccess && cached.Data != null)
                    return new SuccessDataResult<ManufacturerDto>(cached.Data.Adapt<ManufacturerDto>(), _localizer[Messages.Manufacturer_Was_Found]);

                var manufacturer = await _manufacturerRepository.GetByIdAsync(id, tracking: false);
                if (manufacturer == null)
                    return new ErrorDataResult<ManufacturerDto>(_localizer[Messages.Manufacturer_Was_Not_Found]);

                await _cacheService.AddAsync(CacheKeyById(id), manufacturer);
                return new SuccessDataResult<ManufacturerDto>(manufacturer.Adapt<ManufacturerDto>(), _localizer[Messages.Manufacturer_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<ManufacturerDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<ManufacturerDto>>> GetAllAsync()
        {
            try
            {
                var cachedList = await _cacheService.GetListByAsync(CacheKeyAll);
                if (cachedList.IsSuccess && cachedList.Data != null)
                    return new SuccessDataResult<IEnumerable<ManufacturerDto>>(cachedList.Data.Select(x => x.Adapt<ManufacturerDto>()));

                var manufacturers = await _manufacturerRepository.GetAllAsync(tracking: false);
                var list = manufacturers.ToList();
                await _cacheService.AddListAsync(CacheKeyAll, list);

                return new SuccessDataResult<IEnumerable<ManufacturerDto>>(list.Select(x => x.Adapt<ManufacturerDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<ManufacturerDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<ManufacturerDto>> AddAsync(ManufacturerAddDto dto)
        {
            try
            {
                var exists = await _manufacturerRepository.AnyAsync(m => m.Name == dto.Name);
                if (exists)
                    return new ErrorDataResult<ManufacturerDto>(_localizer[Messages.Manufacturer_Name_Already_Exists]);

                IDataResult<ManufacturerDto> result = new ErrorDataResult<ManufacturerDto>(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var manufacturer = new Manufacturer { Name = dto.Name, Country = dto.Country };

                        await _manufacturerRepository.AddAsync(manufacturer);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} Name: {Name}", _localizer[Messages.Manufacturer_HasBeen_Added].Value, dto.Name);
                        result = new SuccessDataResult<ManufacturerDto>(manufacturer.Adapt<ManufacturerDto>(), _localizer[Messages.Manufacturer_HasBeen_Added]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} Name: {Name}", _localizer[Messages.UnexpectedError].Value, dto.Name);
                        result = new ErrorDataResult<ManufacturerDto>(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<ManufacturerDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> UpdateAsync(Guid id, ManufacturerUpdateDto dto)
        {
            try
            {
                var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
                if (manufacturer == null)
                    return new ErrorResult(_localizer[Messages.Manufacturer_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        manufacturer.Name    = dto.Name;
                        manufacturer.Country = dto.Country;

                        await _manufacturerRepository.UpdateAsync(manufacturer);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} ManufacturerId: {Id}", _localizer[Messages.Manufacturer_Was_Updated].Value, id);
                        result = new SuccessResult(_localizer[Messages.Manufacturer_Was_Updated]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} ManufacturerId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
                var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
                if (manufacturer == null)
                    return new ErrorResult(_localizer[Messages.Manufacturer_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        await _manufacturerRepository.DeleteAsync(manufacturer);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _cacheService.DeleteAsync(CacheKeyById(id));
                        await _cacheService.DeleteAsync(CacheKeyAll);

                        _logger.LogInformation("{Message} ManufacturerId: {Id}", _localizer[Messages.Manufacturer_Was_Deleted].Value, id);
                        result = new SuccessResult(_localizer[Messages.Manufacturer_Was_Deleted]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} ManufacturerId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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
