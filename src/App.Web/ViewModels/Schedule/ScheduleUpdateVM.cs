namespace App.Web.ViewModels.Schedule
{
    public class ScheduleUpdateVM
    {
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public List<int> SelectedDays { get; set; } = new();
        public string DepartureTime { get; set; } = "00:00";
        public string TimeZone { get; set; } = string.Empty;
    }
}
