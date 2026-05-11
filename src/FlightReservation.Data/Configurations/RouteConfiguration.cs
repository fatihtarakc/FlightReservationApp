using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightReservation.Data.Configurations;

public class RouteConfiguration : IEntityTypeConfiguration<Core.Entities.Route>
{
    public void Configure(EntityTypeBuilder<Core.Entities.Route> builder)
    {
        builder.ToTable("Routes");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.OriginCity).HasMaxLength(100).IsRequired();
        builder.Property(r => r.OriginCode).HasMaxLength(3).IsRequired();
        builder.Property(r => r.DestinationCity).HasMaxLength(100).IsRequired();
        builder.Property(r => r.DestinationCode).HasMaxLength(3).IsRequired();
        builder.HasIndex(r => new { r.OriginCode, r.DestinationCode }).IsUnique();
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
