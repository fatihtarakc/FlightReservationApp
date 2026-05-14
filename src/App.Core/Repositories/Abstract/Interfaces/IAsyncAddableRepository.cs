namespace App.Core.Repositories.Abstract.Interfaces
{
    public interface IAsyncAddableRepository<Entity> where Entity : AuditableBaseEntity
    {
        ValueTask<Entity> AddAsync(Entity entity);
        Task AddRangeAsync(IEnumerable<Entity> entities);
    }
}
