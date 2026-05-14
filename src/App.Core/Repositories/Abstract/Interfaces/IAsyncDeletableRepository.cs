namespace App.Core.Repositories.Abstract.Interfaces
{
    public interface IAsyncDeletableRepository<Entity> where Entity : AuditableBaseEntity
    {
        ValueTask DeleteAsync(Entity entity);
    }
}
