namespace App.Entities.Configurations.Concrete
{
    public class AdminConfiguration : AuditablePersonBaseEntityConfiguration<Admin>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Admin> builder)
        {
            base.Configure(builder);
            builder.ToTable("admins");

            builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
            builder.ToTable(t => t.HasCheckConstraint("CK_Admin_Name_MinLength",
                "char_length(name) >= 2"));

            builder.Property(a => a.Surname).IsRequired().HasMaxLength(100);
            builder.ToTable(t => t.HasCheckConstraint("CK_Admin_Surname_MinLength",
                "char_length(surname) >= 2"));
        }
    }
}
