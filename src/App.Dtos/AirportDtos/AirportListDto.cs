namespace App.Dtos.AirportDtos
{
    public record AirportListDto(
        Guid Id,
        string Name,
        string IataCode,
        string City,
        string Country);
}
