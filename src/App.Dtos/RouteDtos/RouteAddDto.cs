namespace App.Dtos.RouteDtos
{
    public class RouteAddDto
    {
        public Guid DepartureAirportId { get; set; }
        public Guid ArrivalAirportId { get; set; }
        public decimal DistanceKm { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
    }
}