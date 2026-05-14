namespace App.Entities.Configurations.Concrete
{
    public class RouteConfiguration : AuditableBaseEntityConfiguration<Route>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Route> builder)
        {
            base.Configure(builder);
            builder.ToTable("routes");

            builder.Property(r => r.DistanceKm).HasPrecision(10, 2);
            builder.ToTable(t => t.HasCheckConstraint("CK_Route_DistanceKm_Positive",
                "distance_km > 0"));

            builder.ToTable(t => t.HasCheckConstraint("CK_Route_EstimatedDuration_Positive",
                "estimated_duration > '00:00:00'::interval"));

            builder.HasIndex(r => new { r.DepartureAirportId, r.ArrivalAirportId }).IsUnique();
            builder.ToTable(t => t.HasCheckConstraint("CK_Route_DifferentAirports",
                "departure_airport_id <> arrival_airport_id"));

            builder.HasOne(r => r.DepartureAirport)
                .WithMany(a => a.DepartureRoutes)
                .HasForeignKey(r => r.DepartureAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ArrivalAirport)
                .WithMany(a => a.ArrivalRoutes)
                .HasForeignKey(r => r.ArrivalAirportId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
