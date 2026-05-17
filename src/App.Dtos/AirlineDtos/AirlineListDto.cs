namespace App.Dtos.AirlineDtos
{
    public record AirlineListDto(
        Guid Id,
        string Name,
        string IataCode,
        string Country);
}