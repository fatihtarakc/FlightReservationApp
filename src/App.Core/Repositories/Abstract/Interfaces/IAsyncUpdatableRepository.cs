namespace App.Core.Repositories.Abstract.Interfaces
{
    public interface IAsyncUpdatableRepository<Entity> where Entity : AuditableBaseEntity
    {
        ValueTask<Entity> UpdateAsync(Entity entity);
    }
}
