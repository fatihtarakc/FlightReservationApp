using FlightReservation.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightReservation.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.PnrCode).HasMaxLength(10).IsRequired();
        builder.Property(r => r.PassengerFirstName).HasMaxLength(100).IsRequired();
        builder.Property(r => r.PassengerLastName).HasMaxLength(100).IsRequired();
        builder.Property(r => r.PassengerIdentityNumber).HasMaxLength(20).IsRequired();
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.CancelReason).HasMaxLength(500);
        builder.Ignore(r => r.PassengerFullName);

        // Aynı uçuşta aynı koltuk → unique
        builder.HasIndex(r => new { r.FlightId, r.SeatId }).IsUnique()
            .HasFilter("[Status] != 'Cancelled'");

        builder.HasIndex(r => r.PnrCode).IsUnique();
        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.HasOne(r => r.Flight).WithMany(f => f.Reservations).HasForeignKey(r => r.FlightId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.Seat).WithMany(s => s.Reservations).HasForeignKey(r => r.SeatId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.User).WithMany(u => u.Reservations).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
