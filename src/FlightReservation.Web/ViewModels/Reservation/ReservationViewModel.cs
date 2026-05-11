using FlightReservation.Core.Enums;

namespace FlightReservation.Web.ViewModels.Reservation;

public class ReservationViewModel
{
    public int Id { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public DateTime DepartureUtc { get; set; }
    public DateTime ArrivalUtc { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public SeatClass SeatClass { get; set; }
    public string PassengerFullName { get; set; } = string.Empty;
    public string PassengerIdentityNumber { get; set; } = string.Empty;
    public ReservationStatus Status { get; set; }
    public DateTime ReservedAt { get; set; }
    public string? CancelReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public FlightStatus FlightStatus { get; set; }
    public string? Gate { get; set; }
    public string? Terminal { get; set; }
}

public class CancelReservationViewModel
{
    public int Id { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public DateTime DepartureUtc { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "İptal nedeni gereklidir.")]
    [System.ComponentModel.DataAnnotations.Display(Name = "İptal Nedeni")]
    public string Reason { get; set; } = string.Empty;
}
