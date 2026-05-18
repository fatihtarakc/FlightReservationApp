namespace App.Dtos.AircraftDtos
{
    public record AircraftListDto(
        Guid Id,
        string TailNumber,
        AircraftStatus AircraftStatus,
        Guid AirlineId,
        string AirlineName,
        string ModelName);
}