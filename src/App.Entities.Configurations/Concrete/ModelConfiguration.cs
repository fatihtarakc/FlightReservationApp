namespace App.Entities.Configurations.Concrete
{
    public class ModelConfiguration : AuditableBaseEntityConfiguration<Model>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Model> builder)
        {
            base.Configure(builder);
            builder.ToTable("models");

            builder.HasIndex(m => new { m.ManufacturerId, m.Name }).IsUnique();
            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
            builder.ToTable(t => t.HasCheckConstraint("CK_Model_Name_MinLength",
                "char_length(name) >= 2"));

            builder.Property(m => m.BodyType).IsRequired();

            builder.ToTable(t => t.HasCheckConstraint("CK_Model_MaxPassengerCapacity_Range",
                "max_passenger_capacity BETWEEN 1 AND 1000"));

            builder.ToTable(t => t.HasCheckConstraint("CK_Model_SeatCounts_NonNegative",
                "economy_seats >= 0 AND premium_economy_seats >= 0 AND business_seats >= 0 AND first_class_seats >= 0"));

            builder.Property(m => m.MaxRangeKm).HasPrecision(10, 2);
            builder.ToTable(t => t.HasCheckConstraint("CK_Model_MaxRangeKm_Positive",
                "max_range_km > 0"));

            builder.HasOne(m => m.Manufacturer)
                .WithMany(m => m.Models)
                .HasForeignKey(m => m.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
