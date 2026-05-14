namespace App.Dtos.ScheduleDtos
{
    public class ScheduleAddDto
    {
        public string Code { get; set; } = null!;
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DaysOfWeek DaysOfWeek { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public string TimeZone { get; set; } = null!;
        public Guid RouteId { get; set; }
    }
}
