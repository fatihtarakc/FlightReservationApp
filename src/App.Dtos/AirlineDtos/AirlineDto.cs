namespace App.Dtos.AirlineDtos
{
    public record AirlineDto(
        Guid Id,
        string Name,
        string IataCode,
        string IcaoCode,
        string Country,
        string? LogoUrl,
        string? Website);
}