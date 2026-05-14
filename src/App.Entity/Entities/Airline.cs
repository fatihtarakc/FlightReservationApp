namespace App.Entity.Entities
{
    public class Airline : AuditableBaseEntity
    {
        public Airline()
        {
            Aircrafts = new HashSet<Aircraft>();
            Flights = new HashSet<Flight>();
        }

        public string? Name { get; set; }
        public string? IataCode { get; set; }
        public string? IcaoCode { get; set; }
        public string? Country { get; set; }
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }

        public virtual ICollection<Aircraft> Aircrafts { get; set; }
        public virtual ICollection<Flight> Flights { get; set; }
    }
}
