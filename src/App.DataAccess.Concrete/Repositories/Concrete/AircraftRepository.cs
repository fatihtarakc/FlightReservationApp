namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class AircraftRepository : GenericRepository<Aircraft>, IAircraftRepository
    {
        public AircraftRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<Aircraft> IncludeGetByIdAsync(Guid id, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(a => a.Model).ThenInclude(m => m.Manufacturer)
                .Include(a => a.Airline)
                .Include(a => a.Seats)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<IEnumerable<Aircraft>> GetByAirlineIdAsync(Guid airlineId, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(a => a.Model)
                .Where(a => a.AirlineId == airlineId)
                .ToListAsync();
    }
}
