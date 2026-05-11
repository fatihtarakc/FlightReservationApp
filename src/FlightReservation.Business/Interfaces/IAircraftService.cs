using FlightReservation.Core.Entities;

namespace FlightReservation.Business.Interfaces;

public interface IAircraftService
{
    Task<IEnumerable<Aircraft>> GetAllActiveAsync();
    Task<Aircraft?> GetByIdWithSeatsAsync(int id);
    Task<Aircraft> CreateAsync(Aircraft aircraft);
    Task<Aircraft> UpdateAsync(Aircraft aircraft);
    Task DeleteAsync(int id);
}
