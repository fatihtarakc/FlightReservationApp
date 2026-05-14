namespace App.Dtos.AircraftDtos
{
    public record AircraftDto(
        Guid Id,
        string TailNumber,
        int ManufactureYear,
        AircraftStatus AircraftStatus,
        Guid AirlineId,
        string AirlineName,
        Guid ModelId,
        string ModelName);
}
