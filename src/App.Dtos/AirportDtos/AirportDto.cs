namespace App.Dtos.AirportDtos
{
    public record AirportDto(
        Guid Id,
        string Name,
        string IataCode,
        string IcaoCode,
        string City,
        string Country,
        string TimeZone,
        double Latitude,
        double Longitude);
}