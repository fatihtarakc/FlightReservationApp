namespace App.Dtos.SeatDtos
{
    public record SeatDto(
        Guid Id,
        int Row,
        SeatColumn Column,
        SeatClass SeatClass,
        bool IsWindowSeat,
        bool IsAisleSeat,
        bool HasExtraLegRoom,
        Guid AircraftId,
        bool IsAvailable = false);
}