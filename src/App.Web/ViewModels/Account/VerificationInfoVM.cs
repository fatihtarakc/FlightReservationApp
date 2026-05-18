namespace App.Web.ViewModels.Account
{
    public class VerificationInfoVM
    {
        public string Email { get; set; } = string.Empty;
        public NotificationChannel PreferredChannel { get; set; }
        public string? MaskedPhone { get; set; }
    }
}
