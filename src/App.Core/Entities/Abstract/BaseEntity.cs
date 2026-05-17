namespace App.Core.Entities.Abstract
{
    public abstract class BaseEntity : IBaseEntity
    {
        public Guid Id { get; set; }
        public EntityStatus EntityStatus { get; set; }
    }
}