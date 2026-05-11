using FlightReservation.Core.Entities;
using FlightReservation.Core.Enums;

namespace FlightReservation.Business.Interfaces;

public interface IFlightService
{
    Task<IEnumerable<Flight>> SearchAsync(string origin, string destination, DateTime date);
    Task<Flight?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Flight>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<Flight> CreateAsync(Flight flight);
    Task<Flight> UpdateAsync(Flight flight);
    Task UpdateStatusAsync(int id, FlightStatus status);
    Task DeleteAsync(int id);
    Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int flightId);
    Task<bool> IsSeatAvailableAsync(int flightId, int seatId);
}
