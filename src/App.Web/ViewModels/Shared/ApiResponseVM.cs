namespace App.Web.ViewModels.Shared
{
    public class HomePageVM
    {
        public List<FlightVM> RecentFlights { get; set; } = new();
        public List<RouteVM> PopularRoutes { get; set; } = new();
        public List<AirportVM> Airports { get; set; } = new();
    }

    public class ApiResponseVM<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
