namespace App.Core.Entities.Abstract
{
    public abstract class AuditablePersonBaseEntity : AuditableBaseEntity
    {
        public string Email { get; set; }
        public string IdentityId { get; set; }
    }
}