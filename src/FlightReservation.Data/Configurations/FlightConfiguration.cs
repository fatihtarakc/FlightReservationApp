using FlightReservation.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightReservation.Data.Configurations;

public class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.ToTable("Flights");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.FlightNumber).HasMaxLength(10).IsRequired();
        builder.Property(f => f.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(f => f.Gate).HasMaxLength(10);
        builder.Property(f => f.Terminal).HasMaxLength(10);
        builder.Ignore(f => f.Duration);
        builder.HasIndex(f => f.FlightNumber);
        builder.HasQueryFilter(f => !f.IsDeleted);

        builder.HasOne(f => f.Route).WithMany(r => r.Flights).HasForeignKey(f => f.RouteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(f => f.Aircraft).WithMany(a => a.Flights).HasForeignKey(f => f.AircraftId).OnDelete(DeleteBehavior.Restrict);
    }
}
