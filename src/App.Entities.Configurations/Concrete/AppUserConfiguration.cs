namespace App.Entities.Configurations.Concrete
{
    public class AppUserConfiguration : AuditablePersonBaseEntityConfiguration<AppUser>, IEntityConfiguration
    {
        public override void Configure(EntityTypeBuilder<AppUser> builder)
        {
            base.Configure(builder);
            builder.ToTable("app_users");

            builder.Property(u => u.Name).IsRequired().HasMaxLength(50);
            builder.ToTable(t => t.HasCheckConstraint("CK_AppUser_Name_Pattern",
                "name !~ '[^A-Za-zğüşıöçĞÜŞİÖÇ -]' and char_length(name) >= 2"));

            builder.Property(u => u.Surname).IsRequired().HasMaxLength(50);
            builder.ToTable(t => t.HasCheckConstraint("CK_AppUser_Surname_Pattern",
                "surname !~ '[^A-Za-zğüşıöçĞÜŞİÖÇ -]' and char_length(surname) >= 2"));

            builder.HasIndex(u => u.PhoneNumber).IsUnique();
            builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(16);
            builder.ToTable(t => t.HasCheckConstraint("CK_AppUser_PhoneNumber_Pattern",
                "phone_number ~ '^\\+[0-9]+$' AND char_length(phone_number) >= 8"));

            builder.ToTable(t => t.HasCheckConstraint("CK_AppUser_BirthDate_Age",
                "birth_date <= CURRENT_DATE - INTERVAL '18 years'"));

            builder.Property(u => u.UserStatus).IsRequired()
                .HasDefaultValue(UserStatus.Active);

            builder.Property(u => u.PreferredNotificationChannel).IsRequired()
                .HasDefaultValue(NotificationChannel.Email);

            builder.Property(u => u.PassportNumber).HasMaxLength(20);
            builder.Property(u => u.NationalId).HasMaxLength(20);
            builder.Property(u => u.Nationality).HasMaxLength(100);
        }
    }
}
