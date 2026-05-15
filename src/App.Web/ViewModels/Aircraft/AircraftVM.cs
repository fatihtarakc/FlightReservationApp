using App.Web.Enums;
namespace App.Web.ViewModels.Aircraft
{
    public class AircraftVM
    {
        public Guid Id { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public BodyType BodyType { get; set; }
        public int TotalSeats { get; set; }
        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int ManufactureYear { get; set; }
        public AircraftStatus Status { get; set; }
    }
}
