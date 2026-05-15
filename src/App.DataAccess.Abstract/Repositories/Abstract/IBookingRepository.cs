namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IBookingRepository :
        IAsyncAddableRepository<Booking>, IAsyncDeletableRepository<Booking>,
        IAsyncUpdatableRepository<Booking>, IAsyncQueryableRepository<Booking>,
        IAsyncOrderableRepository<Booking>
    {
        Task<Booking> IncludeGetByIdAsync(Guid id, bool tracking = true);
        Task<Booking> GetByPnrAsync(string pnr, bool tracking = true);
        Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId, bool tracking = false);
        Task<IEnumerable<Booking>> GetActiveBookingsByFlightIdAsync(Guid flightId, bool tracking = false);
        Task<IEnumerable<Booking>> GetPendingRemindersAsync(int hoursBeforeDeparture, bool tracking = true);
        Task<IEnumerable<Booking>> GetAllWithDetailsAsync(bool tracking = false);
    }
}
