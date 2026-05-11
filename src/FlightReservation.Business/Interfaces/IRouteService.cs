using FlightReservation.Core.Entities;

namespace FlightReservation.Business.Interfaces;

public interface IRouteService
{
    Task<IEnumerable<Route>> GetAllActiveAsync();
    Task<Route?> GetByIdAsync(int id);
    Task<Route> CreateAsync(Route route);
    Task<Route> UpdateAsync(Route route);
    Task DeleteAsync(int id);
    Task<IEnumerable<string>> GetCitiesAsync();
}
