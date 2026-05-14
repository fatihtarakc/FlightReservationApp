namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        public FlightRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<Flight> IncludeGetByIdAsync(Guid id, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(f => f.Aircraft).ThenInclude(a => a.Model)
                .Include(f => f.Airline)
                .Include(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.DepartureAirport)
                .Include(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.ArrivalAirport)
                .Include(f => f.Bookings)
                .FirstOrDefaultAsync(f => f.Id == id);

        public async Task<IEnumerable<Flight>> SearchFlightsAsync(string departureIata, string arrivalIata, DateTime departureDate, bool tracking = false) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(f => f.Aircraft).ThenInclude(a => a.Model)
                .Include(f => f.Airline)
                .Include(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.DepartureAirport)
                .Include(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.ArrivalAirport)
                .Include(f => f.Bookings)
                .Where(f =>
                    f.Schedule.Route.DepartureAirport.IataCode == departureIata &&
                    f.Schedule.Route.ArrivalAirport.IataCode == arrivalIata &&
                    f.DepartureDateTime.Date == departureDate.Date &&
                    f.FlightStatus != FlightStatus.Cancelled)
                .OrderBy(f => f.DepartureDateTime)
                .ToListAsync();

        public async Task<IEnumerable<Flight>> GetFlightsDepartingInHoursAsync(int hours, bool tracking = false)
        {
            var now = DateTime.UtcNow;
            var target = now.AddHours(hours);
            return await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.DepartureAirport)
                .Include(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.ArrivalAirport)
                .Include(f => f.Bookings).ThenInclude(b => b.AppUser)
                .Where(f =>
                    f.DepartureDateTime >= now.AddMinutes(-30) &&
                    f.DepartureDateTime <= target.AddMinutes(30) &&
                    f.FlightStatus == FlightStatus.Scheduled)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flight>> GetFlightsDepartingInDaysAsync(int days, bool tracking = false)
        {
            var now = DateTime.UtcNow;
            var target = now.AddDays(days);
            return await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.DepartureAirport)
                .Include(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.ArrivalAirport)
                .Include(f => f.Bookings).ThenInclude(b => b.AppUser)
                .Where(f =>
                    f.DepartureDateTime.Date == target.Date &&
                    f.FlightStatus == FlightStatus.Scheduled)
                .ToListAsync();
        }
    }
}
