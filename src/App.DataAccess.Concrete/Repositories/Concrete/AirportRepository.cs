namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class AirportRepository : GenericRepository<Airport>, IAirportRepository
    {
        public AirportRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<Airport> GetByIataCodeAsync(string iataCode, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .FirstOrDefaultAsync(a => a.IataCode == iataCode);

        public async Task<IEnumerable<string>> GetDistinctCountriesAsync() =>
            await GetAllByStatusIsNotDeletedByTracking(false)
                .Where(a => a.Country != null)
                .Select(a => a.Country!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

        public async Task<IEnumerable<string>> GetDistinctTimezonesAsync() =>
            await GetAllByStatusIsNotDeletedByTracking(false)
                .Where(a => a.TimeZone != null)
                .Select(a => a.TimeZone!)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
    }
}
