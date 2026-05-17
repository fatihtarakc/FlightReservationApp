namespace App.Core.Entities.Abstract.Interfaces
{
    public interface IBaseEntity
    {
        Guid Id { get; set; }
        EntityStatus EntityStatus { get; set; }
    }
}