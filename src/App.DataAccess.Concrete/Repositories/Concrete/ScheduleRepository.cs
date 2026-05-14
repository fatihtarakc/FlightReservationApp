namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<IEnumerable<Schedule>> GetByRouteIdAsync(Guid routeId, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(s => s.Route).ThenInclude(r => r.DepartureAirport)
                .Include(s => s.Route).ThenInclude(r => r.ArrivalAirport)
                .Where(s => s.RouteId == routeId)
                .ToListAsync();
    }
}
