namespace App.Entity.Entities
{
    public class Model : AuditableBaseEntity
    {
        public Model()
        {
            Aircrafts = new HashSet<Aircraft>();
        }

        public string? Name { get; set; }
        public BodyType BodyType { get; set; }
        public int MaxPassengerCapacity { get; set; }
        public int EconomySeats { get; set; }
        public int PremiumEconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int FirstClassSeats { get; set; }
        public decimal MaxRangeKm { get; set; }

        public Guid ManufacturerId { get; set; }
        public virtual Manufacturer? Manufacturer { get; set; }

        public virtual ICollection<Aircraft> Aircrafts { get; set; }
    }
}
