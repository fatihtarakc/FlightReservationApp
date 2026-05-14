namespace App.Entities.Configurations.Concrete
{
    public class SeatConfiguration : AuditableBaseEntityConfiguration<Seat>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Seat> builder)
        {
            base.Configure(builder);
            builder.ToTable("seats");

            builder.HasIndex(s => new { s.AircraftId, s.Row, s.Column }).IsUnique();

            builder.ToTable(t => t.HasCheckConstraint("CK_Seat_Row_Range",
                "row BETWEEN 1 AND 200"));

            builder.Property(s => s.SeatClass).IsRequired()
                .HasDefaultValue(SeatClass.Economy);

            builder.Property(s => s.Column).IsRequired();

            builder.HasOne(s => s.Aircraft)
                .WithMany(a => a.Seats)
                .HasForeignKey(s => s.AircraftId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
