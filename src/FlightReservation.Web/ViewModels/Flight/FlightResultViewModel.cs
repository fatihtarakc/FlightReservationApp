using FlightReservation.Core.Enums;

namespace FlightReservation.Web.ViewModels.Flight;

public class FlightResultViewModel
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string OriginCity { get; set; } = string.Empty;
    public string OriginCode { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public string DestinationCode { get; set; } = string.Empty;
    public DateTime DepartureUtc { get; set; }
    public DateTime ArrivalUtc { get; set; }
    public TimeSpan Duration { get; set; }
    public string AircraftModel { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public FlightStatus Status { get; set; }
    public string? Gate { get; set; }
    public string? Terminal { get; set; }

    public string SearchOrigin { get; set; } = string.Empty;
    public string SearchDestination { get; set; } = string.Empty;
    public DateTime SearchDate { get; set; }
}

public class FlightSearchResultsViewModel
{
    public FlightSearchViewModel Search { get; set; } = new();
    public List<FlightResultViewModel> Flights { get; set; } = [];
}
