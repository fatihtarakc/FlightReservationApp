namespace App.Cache.Services.Concrete
{
    public class CacheService<Entity> : ICacheService<Entity> where Entity : AuditableBaseEntity
    {
        private readonly IDistributedCache distributedCache;
        private readonly IStringLocalizer<MessageResources> stringLocalizer;
        private readonly ILogger<CacheService<Entity>> logger;
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

        public CacheService(IDistributedCache distributedCache, IStringLocalizer<MessageResources> stringLocalizer, ILogger<CacheService<Entity>> logger)
        {
            this.distributedCache = distributedCache;
            this.stringLocalizer = stringLocalizer;
            this.logger = logger;
        }

        public async Task<IDataResult<Entity>> GetByAsync(string cacheKey)
        {
            try
            {
                var json = await distributedCache.GetStringAsync(cacheKey);
                if (json is null) return new ErrorDataResult<Entity>(stringLocalizer[Messages.Redis_Cache_Entity_Was_Not_Found]);

                var entity = JsonSerializer.Deserialize<Entity>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return new SuccessDataResult<Entity>(entity!, stringLocalizer[Messages.Redis_Cache_Entity_Was_Found]);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "{Message} Key: {Key}", stringLocalizer[Messages.Redis_Cache_Entity_Was_Not_Found], cacheKey);
                return new ErrorDataResult<Entity>($"{stringLocalizer[Messages.Redis_Cache_Entity_Was_Not_Found]}: {exception.Message}");
            }
        }

        public async Task<IDataResult<IEnumerable<Entity>>> GetListByAsync(string cacheKey)
        {
            try
            {
                var json = await distributedCache.GetStringAsync(cacheKey);
                if (json is null) return new ErrorDataResult<IEnumerable<Entity>>(stringLocalizer[Messages.Redis_Cache_Entity_Was_Not_Found]);

                var entities = JsonSerializer.Deserialize<IEnumerable<Entity>>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return new SuccessDataResult<IEnumerable<Entity>>(entities!, stringLocalizer[Messages.Redis_Cache_Entity_Was_Found]);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "{Message} Key: {Key}", stringLocalizer[Messages.Redis_Cache_Entity_Was_Not_Found], cacheKey);
                return new ErrorDataResult<IEnumerable<Entity>>($"{stringLocalizer[Messages.Redis_Cache_Entity_Was_Not_Found]}: {exception.Message}");
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
                await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entity, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }), options);
                return new SuccessResult(stringLocalizer[Messages.Redis_Cache_Entity_Was_Added]);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "{Message} Key: {Key}", stringLocalizer[Messages.Redis_Cache_Entity_Could_Not_Be_Added], cacheKey);
                return new ErrorResult($"{stringLocalizer[Messages.Redis_Cache_Entity_Could_Not_Be_Added]}: {exception.Message}");
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
                await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entities, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }), options);
                return new SuccessResult(stringLocalizer[Messages.Redis_Cache_Entity_Was_Added]);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "{Message} Key: {Key}", stringLocalizer[Messages.Redis_Cache_Entity_Could_Not_Be_Added], cacheKey);
                return new ErrorResult($"{stringLocalizer[Messages.Redis_Cache_Entity_Could_Not_Be_Added]}: {exception.Message}");
            }
        }

        public async Task<IResult> DeleteAsync(string cacheKey)
        {
            try
            {
                await distributedCache.RemoveAsync(cacheKey);
                return new SuccessResult(stringLocalizer[Messages.Redis_Cache_Entity_Was_Deleted]);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "{Message} Key: {Key}", stringLocalizer[Messages.Redis_Cache_Entity_Could_Not_Be_Deleted], cacheKey);
                return new ErrorResult($"{stringLocalizer[Messages.Redis_Cache_Entity_Could_Not_Be_Deleted]}: {exception.Message}");
            }
        }
    }
}