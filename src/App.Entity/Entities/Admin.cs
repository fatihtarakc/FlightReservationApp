namespace App.Entity.Entities
{
    public class Admin : AuditablePersonBaseEntity
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
