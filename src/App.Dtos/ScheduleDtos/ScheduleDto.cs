namespace App.Dtos.ScheduleDtos
{
    public record ScheduleDto(
        Guid Id,
        string Code,
        DateTime ValidFrom,
        DateTime? ValidTo,
        DaysOfWeek DaysOfWeek,
        TimeSpan DepartureTime,
        string TimeZone,
        Guid RouteId,
        string DepartureAirport,
        string ArrivalAirport);
}
