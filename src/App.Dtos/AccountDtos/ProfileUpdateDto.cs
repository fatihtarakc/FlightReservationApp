namespace App.Dtos.AccountDtos
{
    public class ProfileUpdateDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public NotificationChannel NotificationPreference { get; set; }
    }
}
