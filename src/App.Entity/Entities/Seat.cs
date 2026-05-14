namespace App.Entity.Entities
{
    public class Seat : AuditableBaseEntity
    {
        public Seat()
        {
            Bookings = new HashSet<Booking>();
        }

        public int Row { get; set; }
        public SeatColumn Column { get; set; }
        public SeatClass SeatClass { get; set; }
        public bool IsWindowSeat { get; set; }
        public bool IsAisleSeat { get; set; }
        public bool HasExtraLegRoom { get; set; }

        public Guid AircraftId { get; set; }
        public virtual Aircraft? Aircraft { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
