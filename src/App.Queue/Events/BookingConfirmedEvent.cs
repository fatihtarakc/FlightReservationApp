namespace App.Queue.Events
{
    public record BookingConfirmedEvent
    {
        public Guid BookingId { get; init; }
        public string? PnrNumber { get; init; }
        public string? PassengerName { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? FlightNumber { get; init; }
        public DateTime DepartureDateTime { get; init; }
        public string? DepartureCity { get; init; }
        public string? ArrivalCity { get; init; }
        public string? SeatNumber { get; init; }
        public SeatClass SeatClass { get; init; }
        public decimal TotalPrice { get; init; }
        public NotificationChannel PreferredChannel { get; init; }
        public string Language { get; init; } = "tr-TR";
    }
}
