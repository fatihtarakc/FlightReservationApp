namespace App.Queue.Events
{
    public record FlightReminderEvent
    {
        public Guid BookingId { get; init; }
        public string? PnrNumber { get; init; }
        public string? PassengerName { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? FlightNumber { get; init; }
        public DateTime DepartureDateTime { get; init; }
        public string? DepartureAirport { get; init; }
        public string? ArrivalAirport { get; init; }
        public NotificationChannel PreferredChannel { get; init; }
        public bool Is7DayReminder { get; init; }
        public string Language { get; init; } = "tr-TR";
    }
}
