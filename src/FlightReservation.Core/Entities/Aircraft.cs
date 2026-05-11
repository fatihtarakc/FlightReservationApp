namespace FlightReservation.Core.Entities;

public class Aircraft : BaseEntity
{
    public string Model { get; set; } = string.Empty;          // Boeing 737-800
    public string Manufacturer { get; set; } = string.Empty;   // Boeing
    public string RegistrationNumber { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int SeatsPerRow { get; set; }                        // genellikle 6 (A-F)
    public int BusinessRowCount { get; set; }                   // ilk N sıra business
    public bool IsActive { get; set; } = true;

    public int TotalCapacity => TotalRows * SeatsPerRow;
    public int BusinessCapacity => BusinessRowCount * SeatsPerRow;
    public int EconomyCapacity => (TotalRows - BusinessRowCount) * SeatsPerRow;

    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
