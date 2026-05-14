namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IRouteRepository :
        IAsyncAddableRepository<Route>, IAsyncDeletableRepository<Route>,
        IAsyncUpdatableRepository<Route>, IAsyncQueryableRepository<Route>,
        IAsyncOrderableRepository<Route>
    {
        Task<Route> IncludeGetByIdAsync(Guid id, bool tracking = true);
        Task<Route> GetByAirportsAsync(Guid departureId, Guid arrivalId, bool tracking = true);
    }
}
