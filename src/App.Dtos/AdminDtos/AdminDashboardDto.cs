namespace App.Dtos.AdminDtos
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalFlights { get; set; }
        public int ActiveFlights { get; set; }
        public int ScheduledFlights { get; set; }
        public int BoardingFlights { get; set; }
        public int DepartedFlights { get; set; }
        public int ArrivedFlights { get; set; }
        public int DelayedFlights { get; set; }
        public int CancelledFlights { get; set; }
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int CheckedInBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int UpcomingFlightsNext24Hours { get; set; }
        public List<FlightListDto> RecentFlights { get; set; } = new();
        public List<BookingDto> RecentBookings { get; set; } = new();
    }

    public class FlightPassengerStatDto
    {
        public Guid FlightId { get; set; }
        public string FlightNumber { get; set; } = null!;
        public DateTime DepartureDateTime { get; set; }
        public string DepartureCity { get; set; } = null!;
        public string ArrivalCity { get; set; } = null!;
        public string FlightStatus { get; set; } = null!;
        public int PassengerCount { get; set; }
        public int TotalCapacity { get; set; }
        public decimal OccupancyRate { get; set; }
    }
}