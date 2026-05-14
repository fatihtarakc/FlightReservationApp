namespace App.Entities.Configurations.Concrete
{
    public class VerificationCodeConfiguration : AuditableBaseEntityConfiguration<VerificationCode>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<VerificationCode> builder)
        {
            base.Configure(builder);
            builder.ToTable("verification_codes");

            builder.Property(v => v.Code).IsRequired().HasMaxLength(6);
            builder.ToTable(t => t.HasCheckConstraint("CK_VerificationCode_Code_Pattern",
                "code ~ '^[0-9]{6}$'"));

            builder.ToTable(t => t.HasCheckConstraint("CK_VerificationCode_ExpiresAt",
                "expires_at > created_date"));

            builder.Property(v => v.AttemptCount).IsRequired()
                .HasDefaultValue(0);
            builder.ToTable(t => t.HasCheckConstraint("CK_VerificationCode_AttemptCount_Range",
                "attempt_count BETWEEN 0 AND 3"));

            builder.Property(v => v.Channel).IsRequired()
                .HasDefaultValue(VerificationCodeChannel.Email);

            builder.Property(v => v.Status).IsRequired()
                .HasDefaultValue(VerificationCodeStatus.Active);

            builder.HasOne(v => v.AppUser)
                .WithMany(u => u.VerificationCodes)
                .HasForeignKey(v => v.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
