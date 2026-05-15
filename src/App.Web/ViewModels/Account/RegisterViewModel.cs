namespace App.Web.ViewModels.Account
{
    public class RegisterViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public NotificationChannel PreferredNotificationChannel { get; set; } = NotificationChannel.Email;
        public string? Nationality { get; set; }
    }
}
