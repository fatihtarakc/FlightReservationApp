namespace App.Dtos.NotificationDtos
{
    public class FlightReminderNotificationDto
    {
        public Guid BookingId { get; set; }
        public string PnrNumber { get; set; } = null!;
        public string PassengerName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string FlightNumber { get; set; } = null!;
        public DateTime DepartureDateTime { get; set; }
        public string DepartureAirport { get; set; } = null!;
        public string ArrivalAirport { get; set; } = null!;
        public NotificationChannel PreferredChannel { get; set; }
        public bool Is7DayReminder { get; set; }
    }
}