namespace App.Dtos.AccountDtos
{
    public class RegisterDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public NotificationChannel PreferredNotificationChannel { get; set; }
        public string? Nationality { get; set; }
    }
}
