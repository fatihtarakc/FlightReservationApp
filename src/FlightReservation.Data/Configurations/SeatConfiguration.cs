using FlightReservation.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightReservation.Data.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("Seats");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.ColumnLetter).HasMaxLength(1).IsRequired();
        builder.Property(s => s.SeatClass).HasConversion<string>().HasMaxLength(20);
        builder.Ignore(s => s.SeatNumber);
        builder.Ignore(s => s.IsWindowSeat);
        builder.Ignore(s => s.IsAisleSeat);
        builder.HasIndex(s => new { s.AircraftId, s.RowNumber, s.ColumnLetter }).IsUnique();
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
