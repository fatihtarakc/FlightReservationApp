using Mapster;
namespace App.Business.Concrete.Services
{
    public class AirlineService : IAirlineService
    {
        private readonly IAirlineRepository _airlineRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService<Airline> _cacheService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<AirlineService> _logger;

        private const string CacheKeyAll = "Airlines:All";
        private static string CacheKeyById(Guid id) => $"Airline:{id}";

        public AirlineService(
            IAirlineRepository airlineRepository,
            IUnitOfWork unitOfWork,
            ICacheService<Airline> cacheService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<AirlineService> logger)
        {
            _airlineRepository = airlineRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<AirlineDto>> GetByIdAsync(Guid id)
        {
            var cached = await _cacheService.GetByAsync(CacheKeyById(id));
            if (cached.IsSuccess && cached.Data != null)
                return new SuccessDataResult<AirlineDto>(cached.Data.Adapt<AirlineDto>(), _localizer[Messages.Airline_Was_Found]);

            var airline = await _airlineRepository.GetByIdAsync(id, tracking: false);
            if (airline == null)
                return new ErrorDataResult<AirlineDto>(_localizer[Messages.Airline_Was_Not_Found]);

            await _cacheService.AddAsync(CacheKeyById(id), airline);
            return new SuccessDataResult<AirlineDto>(airline.Adapt<AirlineDto>(), _localizer[Messages.Airline_Was_Found]);
        }

        public async Task<IDataResult<IEnumerable<AirlineListDto>>> GetAllAsync()
        {
            var cachedList = await _cacheService.GetListByAsync(CacheKeyAll);
            if (cachedList.IsSuccess && cachedList.Data != null)
            {
                var cachedDtos = cachedList.Data.Select(x => x.Adapt<AirlineListDto>());
                return new SuccessDataResult<IEnumerable<AirlineListDto>>(cachedDtos);
            }

            var airlines = await _airlineRepository.GetAllAsync(tracking: false);
            var airlineList = airlines.ToList();
            await _cacheService.AddListAsync(CacheKeyAll, airlineList);

            return new SuccessDataResult<IEnumerable<AirlineListDto>>(airlineList.Select(x => x.Adapt<AirlineListDto>()));
        }

        public async Task<IDataResult<AirlineDto>> AddAsync(AirlineAddDto dto)
        {
            var exists = await _airlineRepository.AnyAsync(a => a.IataCode == dto.IataCode);
            if (exists)
                return new ErrorDataResult<AirlineDto>(_localizer[Messages.Airline_IataCode_Already_Exists]);

            var airline = new Airline
            {
                Name = dto.Name,
                IataCode = dto.IataCode,
                IcaoCode = dto.IcaoCode,
                Country = dto.Country,
                LogoUrl = dto.LogoUrl,
                Website = dto.Website
            };

            await _airlineRepository.AddAsync(airline);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.DeleteAsync(CacheKeyAll);

            _logger.LogInformation("{Message} IataCode: {Code}", _localizer[Messages.Airline_HasBeen_Added].Value, dto.IataCode);
            return new SuccessDataResult<AirlineDto>(airline.Adapt<AirlineDto>(), _localizer[Messages.Airline_HasBeen_Added]);
        }

        public async Task<IResult> UpdateAsync(Guid id, AirlineUpdateDto dto)
        {
            var airline = await _airlineRepository.GetByIdAsync(id);
            if (airline == null)
                return new ErrorResult(_localizer[Messages.Airline_Was_Not_Found]);

            airline.Name = dto.Name;
            airline.IataCode = dto.IataCode;
            airline.IcaoCode = dto.IcaoCode;
            airline.Country = dto.Country;
            airline.LogoUrl = dto.LogoUrl;
            airline.Website = dto.Website;

            await _airlineRepository.UpdateAsync(airline);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.DeleteAsync(CacheKeyById(id));
            await _cacheService.DeleteAsync(CacheKeyAll);

            _logger.LogInformation("{Message} AirlineId: {Id}", _localizer[Messages.Airline_Was_Updated].Value, id);
            return new SuccessResult(_localizer[Messages.Airline_Was_Updated]);
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var airline = await _airlineRepository.GetByIdAsync(id);
            if (airline == null)
                return new ErrorResult(_localizer[Messages.Airline_Was_Not_Found]);

            await _airlineRepository.DeleteAsync(airline);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.DeleteAsync(CacheKeyById(id));
            await _cacheService.DeleteAsync(CacheKeyAll);

            _logger.LogInformation("{Message} AirlineId: {Id}", _localizer[Messages.Airline_Was_Deleted].Value, id);
            return new SuccessResult(_localizer[Messages.Airline_Was_Deleted]);
        }
    }
}





