namespace App.Core.Configurations.Abstract
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : AuditableBaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.EntityStatus).IsRequired()
                .HasDefaultValue(EntityStatus.Added);

            builder.HasQueryFilter(e => e.EntityStatus != EntityStatus.Deleted);
        }
    }
}
