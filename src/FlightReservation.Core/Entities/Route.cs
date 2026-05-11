namespace FlightReservation.Core.Entities;

public class Route : BaseEntity
{
    public string OriginCity { get; set; } = string.Empty;
    public string OriginCode { get; set; } = string.Empty;  // IATA: IST
    public string DestinationCity { get; set; } = string.Empty;
    public string DestinationCode { get; set; } = string.Empty; // IATA: ESB
    public int DistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
