using App.Web.Enums;
namespace App.Web.ViewModels.Aircraft
{
    public class AircraftVM
    {
        public Guid Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tailNumber")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("modelName")]
        public string Model { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("airlineName")]
        public string Manufacturer { get; set; } = string.Empty;

        public BodyType BodyType { get; set; }
        public int TotalSeats { get; set; }
        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int ManufactureYear { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("aircraftStatus")]
        public AircraftStatus Status { get; set; }
    }
}
