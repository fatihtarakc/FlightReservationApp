namespace App.Dtos.SeatDtos
{
    public class SeatAddDto
    {
        public int Row { get; set; }
        public SeatColumn Column { get; set; }
        public SeatClass SeatClass { get; set; }
        public bool IsWindowSeat { get; set; }
        public bool IsAisleSeat { get; set; }
        public bool HasExtraLegRoom { get; set; }
        public Guid AircraftId { get; set; }
    }
}
