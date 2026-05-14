namespace App.Dtos.ModelDtos
{
    public record ModelDto(
        Guid Id,
        string Name,
        BodyType BodyType,
        int MaxPassengerCapacity,
        int EconomySeats,
        int PremiumEconomySeats,
        int BusinessSeats,
        int FirstClassSeats,
        decimal MaxRangeKm,
        string ManufacturerName);
}
