using App.Web.Enums;
namespace App.Web.ViewModels.Aircraft
{
    public class AircraftVM
    {
        public Guid Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tailNumber")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("modelId")]
        public Guid ModelId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("modelName")]
        public string Model { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("airlineId")]
        public Guid AirlineId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("airlineName")]
        public string Manufacturer { get; set; } = string.Empty;

        public int ManufactureYear { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("aircraftStatus")]
        public AircraftStatus Status { get; set; }
    }
}
