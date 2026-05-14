namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface ISeatRepository :
        IAsyncAddableRepository<Seat>, IAsyncDeletableRepository<Seat>,
        IAsyncUpdatableRepository<Seat>, IAsyncQueryableRepository<Seat>,
        IAsyncOrderableRepository<Seat>
    {
        Task<IEnumerable<Seat>> GetAvailableSeatsByFlightIdAsync(Guid flightId, bool tracking = false);
        Task<IEnumerable<Seat>> GetByAircraftIdAsync(Guid aircraftId, bool tracking = false);
    }
}
