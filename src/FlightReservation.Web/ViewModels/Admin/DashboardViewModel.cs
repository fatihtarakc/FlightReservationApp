namespace FlightReservation.Web.ViewModels.Admin;

public class DashboardViewModel
{
    public int TotalFlights { get; set; }
    public int ScheduledFlights { get; set; }
    public int TotalReservations { get; set; }
    public int ActiveReservations { get; set; }
    public int TotalUsers { get; set; }
    public int TotalRoutes { get; set; }
    public int TotalAircrafts { get; set; }
    public List<RecentReservationItem> RecentReservations { get; set; } = [];
    public List<UpcomingFlightItem> UpcomingFlights { get; set; } = [];
}

public class RecentReservationItem
{
    public string PnrCode { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public DateTime ReservedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpcomingFlightItem
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public DateTime DepartureUtc { get; set; }
    public int ReservationCount { get; set; }
    public int TotalSeats { get; set; }
    public string Status { get; set; } = string.Empty;
}
