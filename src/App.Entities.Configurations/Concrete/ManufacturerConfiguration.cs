namespace App.Entities.Configurations.Concrete
{
    public class ManufacturerConfiguration : AuditableBaseEntityConfiguration<Manufacturer>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Manufacturer> builder)
        {
            base.Configure(builder);
            builder.ToTable("manufacturers");

            builder.HasIndex(m => m.Name).IsUnique();
            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
            builder.ToTable(t => t.HasCheckConstraint("CK_Manufacturer_Name_MinLength",
                "char_length(name) >= 2"));

            builder.Property(m => m.Country).IsRequired().HasMaxLength(100);
            builder.ToTable(t => t.HasCheckConstraint("CK_Manufacturer_Country_MinLength",
                "char_length(country) >= 2"));
        }
    }
}
