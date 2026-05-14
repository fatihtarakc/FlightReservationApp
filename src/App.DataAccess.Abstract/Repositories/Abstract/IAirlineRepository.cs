namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IAirlineRepository :
        IAsyncAddableRepository<Airline>, IAsyncDeletableRepository<Airline>,
        IAsyncUpdatableRepository<Airline>, IAsyncQueryableRepository<Airline>,
        IAsyncOrderableRepository<Airline>
    {
        Task<Airline> IncludeGetByIdAsync(Guid id, bool tracking = true);
    }
}
