namespace App.Web.ViewModels.Flight
{
    public class FlightListItemViewModel
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string AirlineName { get; set; } = string.Empty;
        public string DepartureAirportIata { get; set; } = string.Empty;
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalAirportIata { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public decimal BaseEconomyPrice { get; set; }
        public Currency Currency { get; set; }
        public FlightStatus FlightStatus { get; set; }
        public int AvailableSeats { get; set; }
    }
}
