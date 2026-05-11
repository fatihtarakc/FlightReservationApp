using FlightReservation.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightReservation.Data.Configurations;

public class AircraftConfiguration : IEntityTypeConfiguration<Aircraft>
{
    public void Configure(EntityTypeBuilder<Aircraft> builder)
    {
        builder.ToTable("Aircrafts");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Model).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Manufacturer).HasMaxLength(100).IsRequired();
        builder.Property(a => a.RegistrationNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(a => a.RegistrationNumber).IsUnique();
        builder.Ignore(a => a.TotalCapacity);
        builder.Ignore(a => a.BusinessCapacity);
        builder.Ignore(a => a.EconomyCapacity);
        builder.HasQueryFilter(a => !a.IsDeleted);
        builder.HasMany(a => a.Seats).WithOne(s => s.Aircraft).HasForeignKey(s => s.AircraftId).OnDelete(DeleteBehavior.Cascade);
    }
}
