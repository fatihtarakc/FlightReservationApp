namespace App.Entities.Configurations.Concrete
{
    public class AirportConfiguration : AuditableBaseEntityConfiguration<Airport>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Airport> builder)
        {
            base.Configure(builder);
            builder.ToTable("airports");

            builder.HasIndex(a => a.Name).IsUnique();
            builder.Property(a => a.Name).IsRequired().HasMaxLength(150);
            builder.ToTable(t => t.HasCheckConstraint("CK_Airport_Name_MinLength",
                "char_length(name) >= 3"));

            builder.HasIndex(a => a.IataCode).IsUnique();
            builder.Property(a => a.IataCode).IsRequired().HasMaxLength(3);
            builder.ToTable(t => t.HasCheckConstraint("CK_Airport_IataCode_Pattern",
                "iata_code ~ '^[A-Z]{3}$'"));

            builder.HasIndex(a => a.IcaoCode).IsUnique();
            builder.Property(a => a.IcaoCode).IsRequired().HasMaxLength(4);
            builder.ToTable(t => t.HasCheckConstraint("CK_Airport_IcaoCode_Pattern",
                "icao_code ~ '^[A-Z]{4}$'"));

            builder.Property(a => a.City).IsRequired().HasMaxLength(100);
            builder.ToTable(t => t.HasCheckConstraint("CK_Airport_City_MinLength",
                "char_length(city) >= 2"));

            builder.Property(a => a.Country).IsRequired().HasMaxLength(100);
            builder.ToTable(t => t.HasCheckConstraint("CK_Airport_Country_MinLength",
                "char_length(country) >= 2"));

            builder.Property(a => a.TimeZone).IsRequired().HasMaxLength(100);
        }
    }
}
