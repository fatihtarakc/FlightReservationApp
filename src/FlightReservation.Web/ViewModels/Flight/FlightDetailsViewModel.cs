using FlightReservation.Core.Enums;

namespace FlightReservation.Web.ViewModels.Flight;

public class FlightDetailsViewModel
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
    public int TotalRows { get; set; }
    public int SeatsPerRow { get; set; }
    public int BusinessRowCount { get; set; }
    public FlightStatus Status { get; set; }
    public string? Gate { get; set; }
    public string? Terminal { get; set; }
    public List<SeatViewModel> Seats { get; set; } = [];
    public int AvailableEconomy { get; set; }
    public int AvailableBusiness { get; set; }
}

public class SeatViewModel
{
    public int Id { get; set; }
    public int RowNumber { get; set; }
    public string ColumnLetter { get; set; } = string.Empty;
    public string SeatNumber => $"{RowNumber}{ColumnLetter}";
    public SeatClass SeatClass { get; set; }
    public bool IsExitRow { get; set; }
    public bool IsOccupied { get; set; }
    public bool IsWindowSeat => ColumnLetter is "A" or "F" or "J";
    public bool IsAisleSeat => ColumnLetter is "C" or "D" or "G";
}
