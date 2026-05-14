namespace App.Entities.Configurations.Concrete
{
    public class ScheduleConfiguration : AuditableBaseEntityConfiguration<Schedule>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<Schedule> builder)
        {
            base.Configure(builder);
            builder.ToTable("schedules");

            builder.HasIndex(s => s.Code).IsUnique();
            builder.Property(s => s.Code).IsRequired().HasMaxLength(20);
            builder.ToTable(t => t.HasCheckConstraint("CK_Schedule_Code_Pattern",
                "code ~ '^[A-Za-z0-9 .&-]+$' AND char_length(code) >= 3"));

            builder.ToTable(t => t.HasCheckConstraint("CK_Schedule_ValidFromValidTo",
                "valid_to IS NULL OR valid_to > valid_from"));

            builder.Property(s => s.TimeZone).IsRequired().HasMaxLength(100);

            builder.HasOne(s => s.Route)
                .WithMany(r => r.Schedules)
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
