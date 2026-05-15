namespace App.Web.ViewModels.Flight
{
    public class FlightDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public decimal BaseEconomyPrice { get; set; }
        public decimal BasePremiumEconomyPrice { get; set; }
        public decimal BaseBusinessPrice { get; set; }
        public decimal BaseFirstClassPrice { get; set; }
        public Currency Currency { get; set; }
        public FlightStatus FlightStatus { get; set; }
        public string? Gate { get; set; }
        public string? Terminal { get; set; }
        public string AirlineName { get; set; } = string.Empty;
        public string AirlineIata { get; set; } = string.Empty;
        public string AircraftTailNumber { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string DepartureAirportIata { get; set; } = string.Empty;
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalAirportIata { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public int AvailableEconomySeats { get; set; }
        public int AvailableBusinessSeats { get; set; }
        public int AvailableFirstClassSeats { get; set; }
    }
}
