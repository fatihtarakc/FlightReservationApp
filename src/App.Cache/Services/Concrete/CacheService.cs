namespace App.Cache.Services.Concrete
{
    public class CacheService<Entity> : ICacheService<Entity> where Entity : AuditableBaseEntity
    {
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);
        private readonly IDistributedCache _cache;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<CacheService<Entity>> _logger;

        public CacheService(IDistributedCache cache, IStringLocalizer<MessageResources> localizer, ILogger<CacheService<Entity>> logger)
        {
            _cache = cache;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<Entity>> GetByAsync(string cacheKey)
        {
            try
            {
                var json = await _cache.GetStringAsync(cacheKey);
                if (json is null)
                    return new ErrorDataResult<Entity>(_localizer[Messages.Redis_Cache_Entity_Was_Not_Found]);

                var entity = JsonSerializer.Deserialize<Entity>(json);
                return new SuccessDataResult<Entity>(entity!, _localizer[Messages.Redis_Cache_Entity_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message} Key: {Key}", _localizer[Messages.Redis_Cache_Entity_Was_Not_Found], cacheKey);
                return new ErrorDataResult<Entity>($"{_localizer[Messages.Redis_Cache_Entity_Was_Not_Found]}: {ex.Message}");
            }
        }

        public async Task<IDataResult<IEnumerable<Entity>>> GetListByAsync(string cacheKey)
        {
            try
            {
                var json = await _cache.GetStringAsync(cacheKey);
                if (json is null)
                    return new ErrorDataResult<IEnumerable<Entity>>(_localizer[Messages.Redis_Cache_Entity_Was_Not_Found]);

                var entities = JsonSerializer.Deserialize<IEnumerable<Entity>>(json);
                return new SuccessDataResult<IEnumerable<Entity>>(entities!, _localizer[Messages.Redis_Cache_Entity_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message} Key: {Key}", _localizer[Messages.Redis_Cache_Entity_Was_Not_Found], cacheKey);
                return new ErrorDataResult<IEnumerable<Entity>>($"{_localizer[Messages.Redis_Cache_Entity_Was_Not_Found]}: {ex.Message}");
            }
        }

        public async Task<IResult> AddAsync(string cacheKey, Entity entity, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
            };
            try
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entity), options);
                return new SuccessResult(_localizer[Messages.Redis_Cache_Entity_Was_Added]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message} Key: {Key}", _localizer[Messages.Redis_Cache_Entity_Could_Not_Be_Added], cacheKey);
                return new ErrorResult($"{_localizer[Messages.Redis_Cache_Entity_Could_Not_Be_Added]}: {ex.Message}");
            }
        }

        public async Task<IResult> AddListAsync(string cacheKey, IEnumerable<Entity> entities, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
            };
            try
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entities), options);
                return new SuccessResult(_localizer[Messages.Redis_Cache_Entity_Was_Added]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message} Key: {Key}", _localizer[Messages.Redis_Cache_Entity_Could_Not_Be_Added], cacheKey);
                return new ErrorResult($"{_localizer[Messages.Redis_Cache_Entity_Could_Not_Be_Added]}: {ex.Message}");
            }
        }

        public async Task<IResult> DeleteAsync(string cacheKey)
        {
            try
            {
                await _cache.RemoveAsync(cacheKey);
                return new SuccessResult(_localizer[Messages.Redis_Cache_Entity_Was_Deleted]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message} Key: {Key}", _localizer[Messages.Redis_Cache_Entity_Could_Not_Be_Deleted], cacheKey);
                return new ErrorResult($"{_localizer[Messages.Redis_Cache_Entity_Could_Not_Be_Deleted]}: {ex.Message}");
            }
        }
    }
}
