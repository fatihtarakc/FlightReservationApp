namespace App.Web.ViewModels.Admin
{
    public class AdminDashboardVM
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalPassengers { get; set; }
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
        public List<FlightVM> RecentFlights { get; set; } = new();
        public List<BookingVM> RecentBookings { get; set; } = new();
    }
}
