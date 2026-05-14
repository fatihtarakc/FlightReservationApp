namespace App.Core.Entities.Abstract.Interfaces
{
    public interface IUpdatableEntity
    {
        string? ModifiedBy { get; set; }
        DateTime? ModifiedDate { get; set; }
    }
}
