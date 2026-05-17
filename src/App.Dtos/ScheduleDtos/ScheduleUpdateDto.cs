namespace App.Dtos.ScheduleDtos
{
    public class ScheduleUpdateDto
    {
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DaysOfWeek DaysOfWeek { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public string TimeZone { get; set; } = null!;
    }
}