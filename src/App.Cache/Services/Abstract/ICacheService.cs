namespace App.Cache.Services.Abstract
{
    public interface ICacheService<Entity> where Entity : AuditableBaseEntity
    {
        Task<IDataResult<Entity>> GetByAsync(string cacheKey);
        Task<IDataResult<IEnumerable<Entity>>> GetListByAsync(string cacheKey);
        Task<IResult> AddAsync(string cacheKey, Entity entity, TimeSpan? expiration = null);
        Task<IResult> AddListAsync(string cacheKey, IEnumerable<Entity> entities, TimeSpan? expiration = null);
        Task<IResult> DeleteAsync(string cacheKey);
    }
}
