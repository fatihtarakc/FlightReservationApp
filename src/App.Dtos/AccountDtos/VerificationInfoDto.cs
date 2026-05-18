namespace App.Dtos.AccountDtos
{
    public class VerificationInfoDto
    {
        public string Email { get; set; } = null!;
        public NotificationChannel PreferredChannel { get; set; }
        public string? MaskedPhone { get; set; }
    }
}
