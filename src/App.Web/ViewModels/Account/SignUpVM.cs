namespace App.Web.ViewModels.Account
{
    public class SignUpVM
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public NotificationChannel PreferredNotificationChannel { get; set; } = NotificationChannel.Email;
        public string? Nationality { get; set; }
    }
}
