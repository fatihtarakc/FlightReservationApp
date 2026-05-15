namespace App.Web.ViewModels.Flight
{
    public class SeatViewModel
    {
        public Guid Id { get; set; }
        public int Row { get; set; }
        public SeatColumn Column { get; set; }
        public SeatClass SeatClass { get; set; }
        public bool IsWindowSeat { get; set; }
        public bool IsAisleSeat { get; set; }
        public bool HasExtraLegRoom { get; set; }
        public bool IsAvailable { get; set; }
        public string SeatLabel => $"{Row}{Column}";
    }
}
