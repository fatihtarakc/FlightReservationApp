namespace App.Web.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalFlights { get; set; }
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
    }
}
