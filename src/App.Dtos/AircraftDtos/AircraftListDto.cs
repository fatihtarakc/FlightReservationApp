namespace App.Dtos.AircraftDtos
{
    public record AircraftListDto(
        Guid Id,
        string TailNumber,
        AircraftStatus AircraftStatus,
        string AirlineName,
        string ModelName);
}