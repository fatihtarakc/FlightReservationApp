namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class SeatRepository : GenericRepository<Seat>, ISeatRepository
    {
        public SeatRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<IEnumerable<Seat>> GetAvailableSeatsByFlightIdAsync(Guid flightId, bool tracking = false) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Where(s => !s.Bookings.Any(b =>
                    b.FlightId == flightId &&
                    b.BookingStatus != BookingStatus.Cancelled))
                .ToListAsync();

        public async Task<IEnumerable<Seat>> GetByAircraftIdAsync(Guid aircraftId, bool tracking = false) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Where(s => s.AircraftId == aircraftId)
                .OrderBy(s => s.SeatClass)
                .ThenBy(s => s.Row)
                .ThenBy(s => s.Column)
                .ToListAsync();

        public async Task<IEnumerable<Seat>> GetAllByFlightAircraftAsync(Guid flightId, bool tracking = false) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Where(s => s.Aircraft!.Flights.Any(f => f.Id == flightId))
                .OrderBy(s => s.SeatClass)
                .ThenBy(s => s.Row)
                .ThenBy(s => s.Column)
                .ToListAsync();
    }
}
