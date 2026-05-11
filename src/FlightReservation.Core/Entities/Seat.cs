using FlightReservation.Core.Enums;

namespace FlightReservation.Core.Entities;

public class Seat : BaseEntity
{
    public int AircraftId { get; set; }
    public int RowNumber { get; set; }
    public string ColumnLetter { get; set; } = string.Empty;  // A, B, C, D, E, F
    public SeatClass SeatClass { get; set; }
    public bool IsExitRow { get; set; } = false;
    public bool IsWindowSeat => ColumnLetter is "A" or "F";
    public bool IsAisleSeat => ColumnLetter is "C" or "D";

    public string SeatNumber => $"{RowNumber}{ColumnLetter}";

    public Aircraft Aircraft { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
