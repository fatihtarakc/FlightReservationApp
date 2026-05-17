namespace App.Core.Configurations.Abstract
{
    public abstract class AuditablePersonBaseEntityConfiguration<T> : AuditableBaseEntityConfiguration<T> where T : AuditablePersonBaseEntity
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure(builder);

            builder.HasIndex(auditablePersonBaseEntity => auditablePersonBaseEntity.Email).IsUnique();
            builder.Property(auditablePersonBaseEntity => auditablePersonBaseEntity.Email).HasMaxLength(50).IsRequired();
            builder.ToTable(auditablePersonBaseEntity => auditablePersonBaseEntity.HasCheckConstraint($"CK_{typeof(T).Name}_Email_Pattern_Control", "email ~ '@' and char_length(email) >= 5"));

            builder.HasIndex(auditablePersonBaseEntity => auditablePersonBaseEntity.IdentityId).IsUnique();
            builder.Property(auditablePersonBaseEntity => auditablePersonBaseEntity.IdentityId).HasMaxLength(36).IsRequired();
            builder.ToTable(auditablePersonBaseEntity => auditablePersonBaseEntity.HasCheckConstraint($"CK_{typeof(T).Name}_IdentityId_Length_Control", "char_length(identity_id) = 36"));
        }
    }
}