using FlightReservation.Core.Enums;

namespace FlightReservation.Core.Entities;

public class Reservation : BaseEntity
{
    public string PnrCode { get; set; } = string.Empty;
    public int FlightId { get; set; }
    public int SeatId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string PassengerFirstName { get; set; } = string.Empty;
    public string PassengerLastName { get; set; } = string.Empty;
    public string PassengerIdentityNumber { get; set; } = string.Empty;
    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
    public string? CancelReason { get; set; }
    public DateTime? CancelledAt { get; set; }

    public Flight Flight { get; set; } = null!;
    public Seat Seat { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public string PassengerFullName => $"{PassengerFirstName} {PassengerLastName}";
}
