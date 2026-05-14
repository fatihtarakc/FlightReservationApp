namespace App.Entity.Entities
{
    public class VerificationCode : AuditableBaseEntity
    {
        public string? Code { get; set; }
        public VerificationCodeChannel Channel { get; set; }
        public VerificationCodePurpose Purpose { get; set; }
        public VerificationCodeStatus Status { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int AttemptCount { get; set; }

        public Guid AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }
    }
}
