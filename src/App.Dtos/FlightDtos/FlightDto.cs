namespace App.Dtos.FlightDtos
{
    public record FlightDto(
        Guid Id,
        string Number,
        DateTime DepartureDateTime,
        DateTime ArrivalDateTime,
        TimeSpan Duration,
        decimal BaseEconomyPrice,
        decimal BasePremiumEconomyPrice,
        decimal BaseBusinessPrice,
        decimal BaseFirstClassPrice,
        Currency Currency,
        FlightStatus FlightStatus,
        string? Gate,
        string? Terminal,
        string AirlineName,
        string AirlineIata,
        string AircraftTailNumber,
        string ModelName,
        string DepartureAirportIata,
        string DepartureCity,
        string ArrivalAirportIata,
        string ArrivalCity,
        int AvailableEconomySeats,
        int AvailableBusinessSeats,
        int AvailableFirstClassSeats);
}