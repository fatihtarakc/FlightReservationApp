using FlightReservation.Core.Entities;

namespace FlightReservation.Business.Interfaces;

public interface IReservationService
{
    Task<Reservation?> GetByIdAsync(int id);
    Task<Reservation?> GetByPnrAsync(string pnr);
    Task<IEnumerable<Reservation>> GetUserReservationsAsync(string userId);
    Task<IEnumerable<Reservation>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<Reservation> CreateAsync(Reservation reservation);
    Task CancelAsync(int id, string reason, string cancelledByUserId);
    Task<bool> BelongsToUserAsync(int reservationId, string userId);
}
