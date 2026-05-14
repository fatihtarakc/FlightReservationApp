namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IAirportRepository :
        IAsyncAddableRepository<Airport>, IAsyncDeletableRepository<Airport>,
        IAsyncUpdatableRepository<Airport>, IAsyncQueryableRepository<Airport>,
        IAsyncOrderableRepository<Airport>
    {
        Task<Airport> GetByIataCodeAsync(string iataCode, bool tracking = true);
    }
}
