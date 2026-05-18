namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IFlightRepository :
        IAsyncAddableRepository<Flight>, IAsyncDeletableRepository<Flight>,
        IAsyncUpdatableRepository<Flight>, IAsyncQueryableRepository<Flight>,
        IAsyncOrderableRepository<Flight>
    {
        Task<Flight> IncludeGetByIdAsync(Guid id, bool tracking = true);
        Task<IEnumerable<Flight>> SearchFlightsAsync(string departureIata, string arrivalIata, DateTime departureDate, bool tracking = false);
        Task<IEnumerable<Flight>> GetFlightsDepartingInHoursAsync(int hours, bool tracking = false);
        Task<IEnumerable<Flight>> GetFlightsDepartingInDaysAsync(int days, bool tracking = false);
        Task<IEnumerable<Flight>> GetAllWithStatsAsync(bool tracking = false);
        Task<IEnumerable<Flight>> GetUpcomingWithSeatsAsync(DateTime from, bool tracking = false);
    }
}
