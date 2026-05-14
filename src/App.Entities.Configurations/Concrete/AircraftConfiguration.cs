namespace App.Entities.Configurations.Concrete
{
    public class AircraftConfiguration : AuditableBaseEntityConfiguration<Aircraft>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Aircraft> builder)
        {
            base.Configure(builder);
            builder.ToTable("aircrafts");

            builder.HasIndex(a => a.TailNumber).IsUnique();
            builder.Property(a => a.TailNumber).IsRequired().HasMaxLength(10);
            builder.ToTable(t => t.HasCheckConstraint("CK_Aircraft_TailNumber_Pattern",
                "tail_number ~ '^[A-Z0-9-]+$' AND char_length(tail_number) >= 2"));

            builder.Property(a => a.ManufactureYear).IsRequired();
            builder.ToTable(t => t.HasCheckConstraint("CK_Aircraft_ManufactureYear_Range",
                "manufacture_year >= 1950 AND manufacture_year <= EXTRACT(YEAR FROM NOW())::int + 2"));

            builder.Property(a => a.AircraftStatus).IsRequired()
                .HasDefaultValue(AircraftStatus.Active);

            builder.HasOne(a => a.Airline)
                .WithMany(a => a.Aircrafts)
                .HasForeignKey(a => a.AirlineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Model)
                .WithMany(m => m.Aircrafts)
                .HasForeignKey(a => a.ModelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
