namespace App.Entity.Entities
{
    public class AppUser : AuditablePersonBaseEntity
    {
        public AppUser()
        {
            Bookings = new HashSet<Booking>();
            VerificationCodes = new HashSet<VerificationCode>();
        }

        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public UserStatus UserStatus { get; set; }
        public NotificationChannel PreferredNotificationChannel { get; set; }
        public string? PassportNumber { get; set; }
        public string? NationalId { get; set; }
        public string? Nationality { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<VerificationCode> VerificationCodes { get; set; }
    }
}
