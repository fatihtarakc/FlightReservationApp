namespace App.Entity.Entities
{
    public class Aircraft : AuditableBaseEntity
    {
        public Aircraft()
        {
            Flights = new HashSet<Flight>();
            Seats = new HashSet<Seat>();
        }

        public string? TailNumber { get; set; }
        public int ManufactureYear { get; set; }
        public AircraftStatus AircraftStatus { get; set; }

        public Guid AirlineId { get; set; }
        public virtual Airline? Airline { get; set; }

        public Guid ModelId { get; set; }
        public virtual Model? Model { get; set; }

        public virtual ICollection<Flight> Flights { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }
    }
}
