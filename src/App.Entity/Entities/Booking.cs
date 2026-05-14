namespace App.Entity.Entities
{
    public class Booking : AuditableBaseEntity
    {
        public string? PnrNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public Currency Currency { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CheckInTime { get; set; }
        public string? BoardingPassNumber { get; set; }
        public bool IsReminderSent7Days { get; set; }
        public bool IsReminderSent24Hours { get; set; }

        public Guid AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }

        public Guid FlightId { get; set; }
        public virtual Flight? Flight { get; set; }

        public Guid SeatId { get; set; }
        public virtual Seat? Seat { get; set; }
    }
}
