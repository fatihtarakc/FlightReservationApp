namespace App.Web.Models.ViewModels
{
    public class BookViewModel
    {
        public FlightDto Flight { get; set; } = null!;
        public List<SeatDto> AvailableSeats { get; set; } = new();
        public Guid SelectedSeatId { get; set; }
    }
}
