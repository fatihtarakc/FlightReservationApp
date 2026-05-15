namespace App.Core.Configurations.Abstract
{
    public abstract class AuditableBaseEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
        where TEntity : AuditableBaseEntity
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(150);
            builder.ToTable(t => t.HasCheckConstraint(
                $"CK_{typeof(TEntity).Name}_CreatedBy_MinLength",
                "char_length(created_by) >= 5"));

            builder.Property(e => e.CreatedDate).IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(e => e.ModifiedBy).HasMaxLength(150);
            builder.Property(e => e.DeletedBy).HasMaxLength(150);
        }
    }
}
