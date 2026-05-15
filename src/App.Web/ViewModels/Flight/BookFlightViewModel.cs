namespace App.Web.ViewModels.Flight
{
    public class BookFlightViewModel
    {
        public FlightDetailsViewModel Flight { get; set; } = null!;
        public List<SeatViewModel> AllSeats { get; set; } = new();
        public Guid SelectedSeatId { get; set; }

        public BodyType BodyType => AllSeats.Any(s => s.Column > SeatColumn.F) ? BodyType.WideBody : BodyType.NarrowBody;

        public static readonly SeatColumn[] LeftColumns = { SeatColumn.A, SeatColumn.B, SeatColumn.C };
        public static readonly SeatColumn[] MidColumnsNarrow = Array.Empty<SeatColumn>();
        public static readonly SeatColumn[] MidColumnsWide = { SeatColumn.D, SeatColumn.E };
        public static readonly SeatColumn[] RightColumnsNarrow = { SeatColumn.D, SeatColumn.E, SeatColumn.F };
        public static readonly SeatColumn[] RightColumnsWide = { SeatColumn.F, SeatColumn.G, SeatColumn.H, SeatColumn.J, SeatColumn.K };
    }
}
