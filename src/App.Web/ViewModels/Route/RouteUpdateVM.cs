namespace App.Web.ViewModels.Route
{
    public class RouteUpdateVM
    {
        public Guid OriginAirportId { get; set; }
        public Guid DestinationAirportId { get; set; }
        public decimal DistanceKm { get; set; }
        public int EstimatedDuration { get; set; }
    }
}
