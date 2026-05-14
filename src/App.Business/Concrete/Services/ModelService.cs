using Mapster;
namespace App.Business.Concrete.Services
{
    public class ModelService : IModelService
    {
        private readonly IModelRepository _modelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService<Model> _cacheService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<ModelService> _logger;

        private const string CacheKeyAll = "Models:All";
        private static string CacheKeyById(Guid id) => $"Model:{id}";
        private static string CacheKeyByManufacturer(Guid mId) => $"Models:Manufacturer:{mId}";

        public ModelService(
            IModelRepository modelRepository,
            IUnitOfWork unitOfWork,
            ICacheService<Model> cacheService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<ModelService> logger)
        {
            _modelRepository = modelRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<ModelDto>> GetByIdAsync(Guid id)
        {
            var cached = await _cacheService.GetByAsync(CacheKeyById(id));
            if (cached.IsSuccess && cached.Data != null)
                return new SuccessDataResult<ModelDto>(cached.Data.Adapt<ModelDto>(), _localizer[Messages.Model_Was_Found]);

            var model = await _modelRepository.GetByIdAsync(id, tracking: false);
            if (model == null)
                return new ErrorDataResult<ModelDto>(_localizer[Messages.Model_Was_Not_Found]);

            await _cacheService.AddAsync(CacheKeyById(id), model);
            return new SuccessDataResult<ModelDto>(model.Adapt<ModelDto>(), _localizer[Messages.Model_Was_Found]);
        }

        public async Task<IDataResult<IEnumerable<ModelDto>>> GetAllAsync()
        {
            var cachedList = await _cacheService.GetListByAsync(CacheKeyAll);
            if (cachedList.IsSuccess && cachedList.Data != null)
                return new SuccessDataResult<IEnumerable<ModelDto>>(cachedList.Data.Select(x => x.Adapt<ModelDto>()));

            var models = await _modelRepository.GetAllAsync(tracking: false);
            var list = models.ToList();
            await _cacheService.AddListAsync(CacheKeyAll, list);

            return new SuccessDataResult<IEnumerable<ModelDto>>(list.Select(x => x.Adapt<ModelDto>()));
        }

        public async Task<IDataResult<IEnumerable<ModelDto>>> GetByManufacturerIdAsync(Guid manufacturerId)
        {
            var cachedList = await _cacheService.GetListByAsync(CacheKeyByManufacturer(manufacturerId));
            if (cachedList.IsSuccess && cachedList.Data != null)
                return new SuccessDataResult<IEnumerable<ModelDto>>(cachedList.Data.Select(x => x.Adapt<ModelDto>()));

            var models = await _modelRepository.GetAllWhereAsync(m => m.ManufacturerId == manufacturerId, tracking: false);
            var list = models.ToList();
            await _cacheService.AddListAsync(CacheKeyByManufacturer(manufacturerId), list);

            return new SuccessDataResult<IEnumerable<ModelDto>>(list.Select(x => x.Adapt<ModelDto>()));
        }

        public async Task<IDataResult<ModelDto>> AddAsync(ModelAddDto dto)
        {
            var model = new Model
            {
                Name = dto.Name,
                BodyType = dto.BodyType,
                MaxPassengerCapacity = dto.MaxPassengerCapacity,
                EconomySeats = dto.EconomySeats,
                PremiumEconomySeats = dto.PremiumEconomySeats,
                BusinessSeats = dto.BusinessSeats,
                FirstClassSeats = dto.FirstClassSeats,
                MaxRangeKm = dto.MaxRangeKm,
                ManufacturerId = dto.ManufacturerId
            };

            await _modelRepository.AddAsync(model);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.DeleteAsync(CacheKeyAll);
            await _cacheService.DeleteAsync(CacheKeyByManufacturer(dto.ManufacturerId));

            _logger.LogInformation("{Message} Name: {Name}", _localizer[Messages.Model_HasBeen_Added].Value, dto.Name);
            return new SuccessDataResult<ModelDto>(model.Adapt<ModelDto>(), _localizer[Messages.Model_HasBeen_Added]);
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var model = await _modelRepository.GetByIdAsync(id);
            if (model == null)
                return new ErrorResult(_localizer[Messages.Model_Was_Not_Found]);

            await _modelRepository.DeleteAsync(model);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.DeleteAsync(CacheKeyById(id));
            await _cacheService.DeleteAsync(CacheKeyAll);

            _logger.LogInformation("{Message} ModelId: {Id}", _localizer[Messages.Model_Was_Deleted].Value, id);
            return new SuccessResult(_localizer[Messages.Model_Was_Deleted]);
        }
    }
}





