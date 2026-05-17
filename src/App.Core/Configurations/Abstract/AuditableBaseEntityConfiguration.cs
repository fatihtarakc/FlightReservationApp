namespace App.Core.Configurations.Abstract
{
    public abstract class AuditableBaseEntityConfiguration<T> : BaseEntityConfiguration<T> where T : AuditableBaseEntity
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure(builder);

            builder.Property(auditableBaseEntity => auditableBaseEntity.CreatedBy).HasMaxLength(50).IsRequired();
            builder.ToTable(auditableBaseEntity => auditableBaseEntity.HasCheckConstraint($"CK_{typeof(T).Name}_CreatedBy_MinLength_Control", "char_length(created_by) >= 5"));

            builder.Property(auditableBaseEntity => auditableBaseEntity.CreatedDate).HasDefaultValueSql("NOW()").IsRequired();

            builder.Property(auditableBaseEntity => auditableBaseEntity.DeletedBy).HasMaxLength(50);

            builder.Property(auditableBaseEntity => auditableBaseEntity.ModifiedBy).HasMaxLength(50);
        }
    }
}