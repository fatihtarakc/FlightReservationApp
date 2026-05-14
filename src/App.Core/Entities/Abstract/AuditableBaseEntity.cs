namespace App.Core.Entities.Abstract
{
    public abstract class AuditableBaseEntity : BaseEntity, ICreatableEntity, IDeletableEntity, IUpdatableEntity
    {
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
