namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IScheduleRepository :
        IAsyncAddableRepository<Schedule>, IAsyncDeletableRepository<Schedule>,
        IAsyncUpdatableRepository<Schedule>, IAsyncQueryableRepository<Schedule>,
        IAsyncOrderableRepository<Schedule>
    {
        Task<IEnumerable<Schedule>> GetByRouteIdAsync(Guid routeId, bool tracking = true);
    }
}
