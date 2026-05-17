namespace App.Dtos.RouteDtos
{
    public class RouteUpdateDto
    {
        public decimal DistanceKm { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
    }
}