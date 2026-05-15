namespace App.Web.Areas.Admin.ViewModels
{
    public class FlightPassengerStatViewModel
    {
        public Guid FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public DateTime DepartureDateTime { get; set; }
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public string FlightStatus { get; set; } = string.Empty;
        public int PassengerCount { get; set; }
        public int TotalCapacity { get; set; }
        public decimal OccupancyRate { get; set; }
    }
}
