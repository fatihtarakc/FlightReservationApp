namespace App.Web.ViewModels.Schedule
{
    public class ScheduleAddVM
    {
        public string Code { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; } = DateTime.Today;
        public DateTime? ValidTo { get; set; }
        public List<int> SelectedDays { get; set; } = new();
        public string DepartureTime { get; set; } = "00:00";
        public string TimeZone { get; set; } = string.Empty;
        public Guid RouteId { get; set; }
    }
}
