namespace App.Dtos.BookingDtos
{
    public record BookingListDto(
        Guid Id,
        string PnrNumber,
        string FlightNumber,
        DateTime DepartureDateTime,
        string DepartureCity,
        string ArrivalCity,
        string SeatNumber,
        SeatClass SeatClass,
        decimal TotalPrice,
        BookingStatus BookingStatus);
}
