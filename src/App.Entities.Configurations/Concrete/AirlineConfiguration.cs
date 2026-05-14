namespace App.Entities.Configurations.Concrete
{
    public class AirlineConfiguration : AuditableBaseEntityConfiguration<Airline>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Airline> builder)
        {
            base.Configure(builder);
            builder.ToTable("airlines");

            builder.HasIndex(a => a.Name).IsUnique();
            builder.Property(a => a.Name).IsRequired().HasMaxLength(50);
            builder.ToTable(t => t.HasCheckConstraint("CK_Airline_Name_Pattern",
                "name ~ '^[A-Za-z0-9 &-]+$' AND char_length(name) >= 2"));

            builder.HasIndex(a => a.IataCode).IsUnique();
            builder.Property(a => a.IataCode).IsRequired().HasMaxLength(2);
            builder.ToTable(t => t.HasCheckConstraint("CK_Airline_IataCode_Pattern",
                "iata_code ~ '^[A-Z0-9]{2}$'"));

            builder.HasIndex(a => a.IcaoCode).IsUnique();
            builder.Property(a => a.IcaoCode).IsRequired().HasMaxLength(3);
            builder.ToTable(t => t.HasCheckConstraint("CK_Airline_IcaoCode_Pattern",
                "icao_code ~ '^[A-Z]{3}$'"));

            builder.Property(a => a.Country).IsRequired().HasMaxLength(50);
            builder.Property(a => a.LogoUrl).HasMaxLength(500);
            builder.Property(a => a.Website).HasMaxLength(200);
        }
    }
}
