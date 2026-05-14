namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class AirlineRepository : GenericRepository<Airline>, IAirlineRepository
    {
        public AirlineRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<Airline> IncludeGetByIdAsync(Guid id, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(a => a.Aircrafts)
                .FirstOrDefaultAsync(a => a.Id == id);
    }
}
