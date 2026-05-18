namespace App.Web.ViewModels.Flight
{
    public class FlightVM
    {
        public Guid Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("number")]
        public string FlightNumber { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("departureAirportIata")]
        public string OriginIata { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("arrivalAirportIata")]
        public string DestinationIata { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("departureCity")]
        public string OriginCity { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("arrivalCity")]
        public string DestinationCity { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("departureDateTime")]
        public DateTime DepartureTime { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("arrivalDateTime")]
        public DateTime ArrivalTime { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("modelName")]
        public string AircraftModel { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("baseEconomyPrice")]
        public decimal EconomyPrice { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("basePremiumEconomyPrice")]
        public decimal PremiumEconomyPrice { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("baseBusinessPrice")]
        public decimal BusinessPrice { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("baseFirstClassPrice")]
        public decimal FirstClassPrice { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("flightStatus")]
        public FlightStatus Status { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("gate")]
        public string? Gate { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("terminal")]
        public string? Terminal { get; set; }

        public int AvailableEconomySeats { get; set; }
        public int AvailablePremiumEconomySeats { get; set; }
        public int AvailableBusinessSeats { get; set; }
        public int AvailableFirstClassSeats { get; set; }

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
