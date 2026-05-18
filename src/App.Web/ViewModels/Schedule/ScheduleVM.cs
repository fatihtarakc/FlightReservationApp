namespace App.Web.ViewModels.Schedule
{
    public class ScheduleVM
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DaysOfWeek DaysOfWeek { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public string TimeZone { get; set; } = string.Empty;
        public Guid RouteId { get; set; }
        public string DepartureAirport { get; set; } = string.Empty;
        public string ArrivalAirport { get; set; } = string.Empty;
        public string RouteName => $"{DepartureAirport} → {ArrivalAirport}";
    }
}
