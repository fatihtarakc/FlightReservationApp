namespace App.Entity.Entities
{
    public class Flight : AuditableBaseEntity
    {
        public Flight()
        {
            Bookings = new HashSet<Booking>();
        }

        public string? Number { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal BaseEconomyPrice { get; set; }
        public decimal BasePremiumEconomyPrice { get; set; }
        public decimal BaseBusinessPrice { get; set; }
        public decimal BaseFirstClassPrice { get; set; }
        public Currency Currency { get; set; }
        public FlightStatus FlightStatus { get; set; }
        public string? CancellationReason { get; set; }
        public string? Gate { get; set; }
        public string? Terminal { get; set; }

        public Guid AircraftId { get; set; }
        public virtual Aircraft? Aircraft { get; set; }

        public Guid AirlineId { get; set; }
        public virtual Airline? Airline { get; set; }

        public Guid ScheduleId { get; set; }
        public virtual Schedule? Schedule { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
