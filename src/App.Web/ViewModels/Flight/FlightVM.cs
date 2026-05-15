namespace App.Web.ViewModels.Flight
{
    public class FlightVM
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string OriginIata { get; set; } = string.Empty;
        public string DestinationIata { get; set; } = string.Empty;
        public string OriginCity { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string AircraftModel { get; set; } = string.Empty;
        public decimal EconomyPrice { get; set; }
        public decimal? PremiumEconomyPrice { get; set; }
        public decimal? BusinessPrice { get; set; }
        public decimal? FirstClassPrice { get; set; }
        public FlightStatus Status { get; set; }
        public int AvailableEconomySeats { get; set; }
        public int AvailableBusinessSeats { get; set; }

        public int DurationMinutes => (int)(ArrivalTime - DepartureTime).TotalMinutes;
        public string DurationFormatted => $"{DurationMinutes / 60}s {DurationMinutes % 60}dk";
    }

    public class FlightDetailPageVM
    {
        public FlightVM Flight { get; set; } = new();
        public List<SeatVM> Seats { get; set; } = new();
        public bool IsLoggedIn { get; set; }
    }
}
