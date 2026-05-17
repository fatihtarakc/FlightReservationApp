namespace App.Dtos.RouteDtos
{
    public record RouteDto(
        Guid Id,
        decimal DistanceKm,
        TimeSpan EstimatedDuration,
        string DepartureAirportIata,
        string DepartureCity,
        string ArrivalAirportIata,
        string ArrivalCity);
}