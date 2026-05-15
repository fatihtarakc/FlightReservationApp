namespace App.Web.ViewModels.Booking
{
    public class BookingDetailsViewModel
    {
        public Guid Id { get; set; }
        public string PnrNumber { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public Currency Currency { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public DateTime? CheckInTime { get; set; }
        public string? BoardingPassNumber { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public DateTime DepartureDateTime { get; set; }
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public SeatClass SeatClass { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
