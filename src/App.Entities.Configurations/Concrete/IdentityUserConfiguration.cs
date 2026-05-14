using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Entities.Configurations.Concrete
{
    public class IdentityUserConfiguration : IEntityTypeConfiguration<IdentityUser>, IEntityConfiguration
    {
        public void Configure(EntityTypeBuilder<IdentityUser> builder)
        {
            builder.ToTable("identity_users");

            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Email).HasMaxLength(100);
            builder.ToTable(t => t.HasCheckConstraint("CK_IdentityUser_Email_MinLength",
                "char_length(email) >= 5 AND email ~ '@'"));

            builder.Property(u => u.NormalizedEmail).HasMaxLength(100);
            builder.Property(u => u.UserName).HasMaxLength(256);
            builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
        }
    }

    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>, IEntityConfiguration
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.ToTable("identity_roles");
        }
    }
}
