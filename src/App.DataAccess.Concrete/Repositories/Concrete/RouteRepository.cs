namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class RouteRepository : GenericRepository<Route>, IRouteRepository
    {
        public RouteRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<Route> IncludeGetByIdAsync(Guid id, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(r => r.DepartureAirport)
                .Include(r => r.ArrivalAirport)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Route> GetByAirportsAsync(Guid departureId, Guid arrivalId, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .FirstOrDefaultAsync(r => r.DepartureAirportId == departureId && r.ArrivalAirportId == arrivalId);
    }
}
