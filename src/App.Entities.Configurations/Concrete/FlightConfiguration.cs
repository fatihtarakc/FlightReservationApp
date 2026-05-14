namespace App.Entities.Configurations.Concrete
{
    public class FlightConfiguration : AuditableBaseEntityConfiguration<Flight>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Flight> builder)
        {
            base.Configure(builder);
            builder.ToTable("flights");

            builder.HasIndex(f => new { f.Number, f.DepartureDateTime }).IsUnique();
            builder.Property(f => f.Number).IsRequired().HasMaxLength(6);
            builder.ToTable(t => t.HasCheckConstraint("CK_Flight_Number_Pattern",
                "number ~ '^[A-Z0-9]{3,6}$'"));

            builder.ToTable(t => t.HasCheckConstraint("CK_Flight_Duration",
                "arrival_date_time > departure_date_time"));

            builder.Property(f => f.BaseEconomyPrice).HasPrecision(12, 2);
            builder.Property(f => f.BasePremiumEconomyPrice).HasPrecision(12, 2);
            builder.Property(f => f.BaseBusinessPrice).HasPrecision(12, 2);
            builder.Property(f => f.BaseFirstClassPrice).HasPrecision(12, 2);
            builder.ToTable(t => t.HasCheckConstraint("CK_Flight_Prices_Positive",
                "base_economy_price > 0 AND base_premium_economy_price > 0 AND base_business_price > 0 AND base_first_class_price > 0"));

            builder.Property(f => f.Currency).IsRequired()
                .HasDefaultValue(Currency.TRY);

            builder.Property(f => f.FlightStatus).IsRequired()
                .HasDefaultValue(FlightStatus.Scheduled);

            builder.Property(f => f.Gate).HasMaxLength(10);
            builder.Property(f => f.Terminal).HasMaxLength(10);
            builder.Property(f => f.CancellationReason).HasMaxLength(500);

            builder.HasOne(f => f.Aircraft)
                .WithMany(a => a.Flights)
                .HasForeignKey(f => f.AircraftId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Airline)
                .WithMany(a => a.Flights)
                .HasForeignKey(f => f.AirlineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Schedule)
                .WithMany(s => s.Flights)
                .HasForeignKey(f => f.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

