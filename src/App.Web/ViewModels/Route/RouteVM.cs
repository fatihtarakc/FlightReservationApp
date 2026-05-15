namespace App.Web.ViewModels.Route
{
    public class RouteVM
    {
        public Guid Id { get; set; }
        public decimal DistanceKm { get; set; }
        public int EstimatedDuration { get; set; }
        public string OriginIata { get; set; } = null!;
        public string OriginCity { get; set; } = null!;
        public string DestinationIata { get; set; } = null!;
        public string DestinationCity { get; set; } = null!;
    }
}
