namespace App.Entity.Entities
{
    public class Airport : AuditableBaseEntity
    {
        public Airport()
        {
            DepartureRoutes = new HashSet<Route>();
            ArrivalRoutes = new HashSet<Route>();
        }

        public string? Name { get; set; }
        public string? IataCode { get; set; }
        public string? IcaoCode { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? TimeZone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public virtual ICollection<Route> DepartureRoutes { get; set; }
        public virtual ICollection<Route> ArrivalRoutes { get; set; }
    }
}
