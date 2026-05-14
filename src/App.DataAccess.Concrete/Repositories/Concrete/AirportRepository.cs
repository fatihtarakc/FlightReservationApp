namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class AirportRepository : GenericRepository<Airport>, IAirportRepository
    {
        public AirportRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<Airport> GetByIataCodeAsync(string iataCode, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .FirstOrDefaultAsync(a => a.IataCode == iataCode);
    }
}
