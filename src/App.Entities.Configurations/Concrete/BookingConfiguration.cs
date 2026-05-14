namespace App.Entities.Configurations.Concrete
{
    public class BookingConfiguration : AuditableBaseEntityConfiguration<Booking>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Booking> builder)
        {
            base.Configure(builder);
            builder.ToTable("bookings");

            builder.HasIndex(b => b.PnrNumber).IsUnique();
            builder.Property(b => b.PnrNumber).IsRequired().HasMaxLength(6);
            builder.ToTable(t => t.HasCheckConstraint("CK_Booking_PnrNumber_Pattern",
                "pnr_number ~ '^[A-Z0-9]{6}$'"));

            builder.Property(b => b.TotalPrice).HasPrecision(12, 2);
            builder.ToTable(t => t.HasCheckConstraint("CK_Booking_TotalPrice_Positive",
                "total_price > 0"));

            builder.Property(b => b.BookingStatus).IsRequired()
                .HasDefaultValue(BookingStatus.Pending);

            builder.Property(b => b.CancellationReason).HasMaxLength(500);
            builder.Property(b => b.BoardingPassNumber).HasMaxLength(50);

            builder.HasIndex(b => new { b.FlightId, b.SeatId })
                .IsUnique()
                .HasFilter("booking_status != 3"); // excludes Cancelled bookings so the seat can be rebooked

            builder.HasOne(b => b.AppUser)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Flight)
                .WithMany(f => f.Bookings)
                .HasForeignKey(b => b.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Seat)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

