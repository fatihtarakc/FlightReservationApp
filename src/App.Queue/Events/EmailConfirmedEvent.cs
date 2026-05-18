namespace App.Queue.Events
{
    public record EmailConfirmedEvent
    {
        public string? Name { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public NotificationChannel PreferredChannel { get; init; }
        public string Language { get; init; } = "tr-TR";
    }
}
