using App.Web.Enums;
namespace App.Web.ViewModels.Booking
{
    public class BookingVM
    {
        public Guid Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("pnrNumber")]
        public string BookingCode { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }
        public Currency Currency { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("bookingStatus")]
        public BookingStatus Status { get; set; }

        public DateTime? CheckInTime { get; set; }
        public string? BoardingPassNumber { get; set; }
        public Guid AppUserId { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerEmail { get; set; } = string.Empty;
        public Guid FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("departureCity")]
        public string OriginIata { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("arrivalCity")]
        public string DestinationIata { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("departureDateTime")]
        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }
        public Guid SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public SeatClass SeatClass { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("createdDate")]
        public DateTime BookingDate { get; set; }
    }
}
