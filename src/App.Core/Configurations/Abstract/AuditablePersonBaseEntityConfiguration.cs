using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Core.Configurations.Abstract
{
    public abstract class AuditablePersonBaseEntityConfiguration<TEntity> : AuditableBaseEntityConfiguration<TEntity>
        where TEntity : AuditablePersonBaseEntity
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            base.Configure(builder);

            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
            builder.ToTable(t => t.HasCheckConstraint(
                $"CK_{typeof(TEntity).Name}_Email_MinLength",
                "char_length(email) >= 5 AND email ~ '@'"));

            builder.HasIndex(e => e.IdentityId).IsUnique();
            builder.Property(e => e.IdentityId).IsRequired().HasMaxLength(450);
        }
    }
}
