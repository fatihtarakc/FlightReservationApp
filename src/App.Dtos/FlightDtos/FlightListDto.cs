namespace App.Dtos.FlightDtos
{
    public record FlightListDto(
        Guid Id,
        string Number,
        DateTime DepartureDateTime,
        DateTime ArrivalDateTime,
        string AirlineName,
        string DepartureAirportIata,
        string DepartureCity,
        string ArrivalAirportIata,
        string ArrivalCity,
        decimal BaseEconomyPrice,
        Currency Currency,
        FlightStatus FlightStatus,
        int AvailableSeats);
}
