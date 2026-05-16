namespace App.Queue.Events
{
    public record FlightCancelledEvent
    {
        public Guid FlightId { get; init; }
        public string? FlightNumber { get; init; }
        public DateTime DepartureDateTime { get; init; }
        public string? DepartureCity { get; init; }
        public string? ArrivalCity { get; init; }
        public string? CancellationReason { get; init; }
        public IEnumerable<AffectedPassenger> AffectedPassengers { get; init; } = new List<AffectedPassenger>();
        public string Language { get; init; } = "tr-TR";
    }

    public record AffectedPassenger
    {
        public string? PnrNumber { get; init; }
        public string? Name { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public NotificationChannel PreferredChannel { get; init; }
    }
}

