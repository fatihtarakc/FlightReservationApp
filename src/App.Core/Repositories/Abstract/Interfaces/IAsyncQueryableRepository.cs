namespace App.Core.Repositories.Abstract.Interfaces
{
    public interface IAsyncQueryableRepository<Entity> where Entity : AuditableBaseEntity
    {
        Task<bool> AnyAsync(Expression<Func<Entity, bool>> expression = null!);
        Task<Entity> GetByIdAsync(Guid entityId, bool tracking = true);
        Task<Entity> GetFirstOrDefaultAsync(Expression<Func<Entity, bool>> expression, bool tracking = true);
        Task<IEnumerable<Entity>> GetAllWhereAsync(Expression<Func<Entity, bool>> expression, bool tracking = true);
        Task<IEnumerable<Entity>> GetAllAsync(bool tracking = true);
    }
}
