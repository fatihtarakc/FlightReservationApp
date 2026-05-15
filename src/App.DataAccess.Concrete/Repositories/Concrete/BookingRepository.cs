namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<Booking> IncludeGetByIdAsync(Guid id, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(b => b.AppUser)
                .Include(b => b.Flight).ThenInclude(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.ArrivalAirport)
                .Include(b => b.Flight).ThenInclude(f => f.Airline)
                .Include(b => b.Seat)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<Booking> GetByPnrAsync(string pnr, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(b => b.AppUser)
                .Include(b => b.Flight).ThenInclude(f => f.Airline)
                .Include(b => b.Seat)
                .FirstOrDefaultAsync(b => b.PnrNumber == pnr);

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId, bool tracking = false) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(b => b.Flight).ThenInclude(f => f.Airline)
                .Include(b => b.Flight).ThenInclude(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.ArrivalAirport)
                .Include(b => b.Seat)
                .Where(b => b.AppUserId == userId)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

        public async Task<IEnumerable<Booking>> GetActiveBookingsByFlightIdAsync(Guid flightId, bool tracking = false) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(b => b.AppUser)
                .Include(b => b.Seat)
                .Where(b => b.FlightId == flightId && b.BookingStatus != BookingStatus.Cancelled)
                .ToListAsync();

        public async Task<IEnumerable<Booking>> GetAllWithDetailsAsync(bool tracking = false) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(b => b.AppUser)
                .Include(b => b.Flight).ThenInclude(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.ArrivalAirport)
                .Include(b => b.Seat)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

        public async Task<IEnumerable<Booking>> GetPendingRemindersAsync(int hoursBeforeDeparture, bool tracking = true)
        {
            var now = DateTime.UtcNow;
            var targetTime = now.AddHours(hoursBeforeDeparture);

            return await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Include(b => b.AppUser)
                .Include(b => b.Flight).ThenInclude(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.Schedule).ThenInclude(s => s.Route).ThenInclude(r => r.ArrivalAirport)
                .Include(b => b.Seat)
                .Where(b =>
                    b.BookingStatus == BookingStatus.Confirmed &&
                    b.Flight.DepartureDateTime >= now &&
                    b.Flight.DepartureDateTime <= targetTime.AddHours(1) &&
                    b.Flight.FlightStatus == FlightStatus.Scheduled &&
                    (hoursBeforeDeparture >= 150 ? !b.IsReminderSent7Days : !b.IsReminderSent24Hours))
                .ToListAsync();
        }
    }
}
