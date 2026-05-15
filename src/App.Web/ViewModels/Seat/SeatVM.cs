using App.Web.Enums;
namespace App.Web.ViewModels.Seat
{
    public class SeatVM
    {
        public Guid Id { get; set; }
        public int Row { get; set; }
        public SeatColumn Column { get; set; }
        public SeatClass SeatClass { get; set; }
        public bool IsWindowSeat { get; set; }
        public bool IsAisleSeat { get; set; }
        public bool HasExtraLegRoom { get; set; }
        public Guid AircraftId { get; set; }
        public bool IsAvailable { get; set; }
        public string SeatNumber => $"{Row}{Column}";
    }
}
