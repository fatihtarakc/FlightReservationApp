namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IAircraftRepository :
        IAsyncAddableRepository<Aircraft>, IAsyncDeletableRepository<Aircraft>,
        IAsyncUpdatableRepository<Aircraft>, IAsyncQueryableRepository<Aircraft>,
        IAsyncOrderableRepository<Aircraft>
    {
        Task<Aircraft> IncludeGetByIdAsync(Guid id, bool tracking = true);
        Task<IEnumerable<Aircraft>> GetByAirlineIdAsync(Guid airlineId, bool tracking = true);
    }
}
