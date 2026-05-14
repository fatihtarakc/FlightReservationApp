namespace App.Core.Entities.Abstract.Interfaces
{
    public interface IDeletableEntity
    {
        string? DeletedBy { get; set; }
        DateTime? DeletedDate { get; set; }
    }
}
