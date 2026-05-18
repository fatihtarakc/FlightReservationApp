namespace App.Web.ViewModels.Route
{
    public class RouteVM
    {
        public Guid Id { get; set; }
        public decimal DistanceKm { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("estimatedDuration")]
        public TimeSpan EstimatedDuration { get; set; }

        public int DurationMinutes => (int)EstimatedDuration.TotalMinutes;

        [System.Text.Json.Serialization.JsonPropertyName("departureAirportIata")]
        public string OriginIata { get; set; } = null!;

        [System.Text.Json.Serialization.JsonPropertyName("departureCity")]
        public string OriginCity { get; set; } = null!;

        [System.Text.Json.Serialization.JsonPropertyName("arrivalAirportIata")]
        public string DestinationIata { get; set; } = null!;

        [System.Text.Json.Serialization.JsonPropertyName("arrivalCity")]
        public string DestinationCity { get; set; } = null!;

        public int FlightCount { get; set; }
    }
}
