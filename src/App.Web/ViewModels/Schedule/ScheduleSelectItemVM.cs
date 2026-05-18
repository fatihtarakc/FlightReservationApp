using System.Text.Json.Serialization;

namespace App.Web.ViewModels.Schedule
{
    public class ScheduleSelectItemVM
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("departureAirport")]
        public string DepartureAirport { get; set; } = string.Empty;

        [JsonPropertyName("arrivalAirport")]
        public string ArrivalAirport { get; set; } = string.Empty;

        [JsonPropertyName("routeId")]
        public Guid RouteId { get; set; }

        public int DurationMinutes { get; set; }
    }
}
