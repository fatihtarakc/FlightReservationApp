using FlightReservation.Core.Enums;

namespace FlightReservation.Core.Entities;

public class Flight : BaseEntity
{
    public string FlightNumber { get; set; } = string.Empty;  // TK2026
    public int RouteId { get; set; }
    public int AircraftId { get; set; }
    public DateTime DepartureUtc { get; set; }
    public DateTime ArrivalUtc { get; set; }
    public FlightStatus Status { get; set; } = FlightStatus.Scheduled;
    public string? Gate { get; set; }
    public string? Terminal { get; set; }

    public Route Route { get; set; } = null!;
    public Aircraft Aircraft { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public TimeSpan Duration => ArrivalUtc - DepartureUtc;
}
