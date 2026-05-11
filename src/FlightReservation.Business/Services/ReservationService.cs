using FlightReservation.Business.Interfaces;
using FlightReservation.Core.Entities;
using FlightReservation.Core.Enums;
using FlightReservation.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Business.Services;

public class ReservationService : IReservationService
{
    private readonly AppDbContext _db;

    public ReservationService(AppDbContext db) => _db = db;

    public async Task<Reservation?> GetByIdAsync(int id) =>
        await _db.Reservations
            .Include(r => r.Flight).ThenInclude(f => f.Route)
            .Include(r => r.Seat)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Reservation?> GetByPnrAsync(string pnr) =>
        await _db.Reservations
            .Include(r => r.Flight).ThenInclude(f => f.Route)
            .Include(r => r.Seat)
            .FirstOrDefaultAsync(r => r.PnrCode == pnr.ToUpper());

    public async Task<IEnumerable<Reservation>> GetUserReservationsAsync(string userId) =>
        await _db.Reservations
            .Include(r => r.Flight).ThenInclude(f => f.Route)
            .Include(r => r.Seat)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.ReservedAt)
            .ToListAsync();

    public async Task<IEnumerable<Reservation>> GetAllAsync(int page = 1, int pageSize = 20) =>
        await _db.Reservations
            .Include(r => r.Flight).ThenInclude(f => f.Route)
            .Include(r => r.Seat)
            .Include(r => r.User)
            .OrderByDescending(r => r.ReservedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<Reservation> CreateAsync(Reservation reservation)
    {
        // Koltuk hâlâ boş mu kontrol et (race condition için son kontrol)
        var alreadyTaken = await _db.Reservations.AnyAsync(r =>
            r.FlightId == reservation.FlightId &&
            r.SeatId == reservation.SeatId &&
            r.Status == ReservationStatus.Active);

        if (alreadyTaken)
            throw new InvalidOperationException("Bu koltuk zaten rezerve edilmiş.");

        reservation.PnrCode = GeneratePnr();
        reservation.ReservedAt = DateTime.UtcNow;
        reservation.Status = ReservationStatus.Active;

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();
        return reservation;
    }

    public async Task CancelAsync(int id, string reason, string cancelledByUserId)
    {
        var reservation = await _db.Reservations.FindAsync(id)
            ?? throw new KeyNotFoundException($"Rezervasyon {id} bulunamadı.");

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("Rezervasyon zaten iptal edilmiş.");

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelReason = reason;
        reservation.CancelledAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    public async Task<bool> BelongsToUserAsync(int reservationId, string userId) =>
        await _db.Reservations.AnyAsync(r => r.Id == reservationId && r.UserId == userId);

    private static string GeneratePnr()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        return new string(Enumerable.Range(0, 6)
            .Select(_ => chars[Random.Shared.Next(chars.Length)])
            .ToArray());
    }
}
