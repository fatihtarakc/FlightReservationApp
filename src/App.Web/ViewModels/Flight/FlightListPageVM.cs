namespace App.Web.ViewModels.Flight
{
    public class FlightListPageVM
    {
        public FlightSearchVM SearchVM { get; set; } = new();
        public List<FlightVM> Flights { get; set; } = new();
        public List<AirportVM> Airports { get; set; } = new();
        public bool IsSearchResult { get; set; }
        public bool IsRouteResult { get; set; }
        public bool IsLoggedIn { get; set; }
    }

}
