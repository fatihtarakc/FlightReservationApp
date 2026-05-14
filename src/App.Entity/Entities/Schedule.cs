namespace App.Entity.Entities
{
    public class Schedule : AuditableBaseEntity
    {
        public Schedule()
        {
            Flights = new HashSet<Flight>();
        }

        public string? Code { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DaysOfWeek DaysOfWeek { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public string? TimeZone { get; set; }

        public Guid RouteId { get; set; }
        public virtual Route? Route { get; set; }

        public virtual ICollection<Flight> Flights { get; set; }
    }
}
