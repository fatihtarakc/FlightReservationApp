namespace App.Dtos.BookingDtos
{
    public record BookingDto(
        Guid Id,
        string PnrNumber,
        decimal TotalPrice,
        Currency Currency,
        BookingStatus BookingStatus,
        DateTime? CheckInTime,
        string? BoardingPassNumber,
        Guid AppUserId,
        string PassengerName,
        Guid FlightId,
        string FlightNumber,
        DateTime DepartureDateTime,
        string DepartureCity,
        string ArrivalCity,
        string SeatNumber,
        SeatClass SeatClass,
        DateTime CreatedDate);
}