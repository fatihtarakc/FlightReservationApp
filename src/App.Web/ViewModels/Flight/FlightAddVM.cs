using App.Web.Enums;
namespace App.Web.ViewModels.Flight
{
    public class FlightAddVM
    {
        public string FlightNumber { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal EconomyPrice { get; set; }
        public decimal? PremiumEconomyPrice { get; set; }
        public decimal? BusinessPrice { get; set; }
        public decimal? FirstClassPrice { get; set; }
        public Currency Currency { get; set; } = Currency.TRY;
        public string? Gate { get; set; }
        public string? Terminal { get; set; }
        public Guid AircraftId { get; set; }
        public Guid AirlineId { get; set; }
        public Guid ScheduleId { get; set; }
    }
}
