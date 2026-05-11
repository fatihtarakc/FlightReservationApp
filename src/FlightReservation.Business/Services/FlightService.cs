using FlightReservation.Business.Interfaces;
using FlightReservation.Core.Entities;
using FlightReservation.Core.Enums;
using FlightReservation.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Business.Services;

public class FlightService : IFlightService
{
    private readonly AppDbContext _db;

    public FlightService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Flight>> SearchAsync(string origin, string destination, DateTime date)
    {
        var utcDate = date.ToUniversalTime().Date;

        return await _db.Flights
            .Include(f => f.Route)
            .Include(f => f.Aircraft)
            .Include(f => f.Reservations)
            .Where(f =>
                f.Route.OriginCode == origin.ToUpper() &&
                f.Route.DestinationCode == destination.ToUpper() &&
                f.DepartureUtc.Date == utcDate &&
                f.Status == FlightStatus.Scheduled)
            .OrderBy(f => f.DepartureUtc)
            .ToListAsync();
    }

    public async Task<Flight?> GetByIdWithDetailsAsync(int id) =>
        await _db.Flights
            .Include(f => f.Route)
            .Include(f => f.Aircraft).ThenInclude(a => a.Seats)
            .Include(f => f.Reservations.Where(r => r.Status == ReservationStatus.Active))
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<IEnumerable<Flight>> GetAllAsync(int page = 1, int pageSize = 20) =>
        await _db.Flights
            .Include(f => f.Route)
            .Include(f => f.Aircraft)
            .OrderByDescending(f => f.DepartureUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<Flight> CreateAsync(Flight flight)
    {
        _db.Flights.Add(flight);
        await _db.SaveChangesAsync();
        return flight;
    }

    public async Task<Flight> UpdateAsync(Flight flight)
    {
        _db.Flights.Update(flight);
        await _db.SaveChangesAsync();
        return flight;
    }

    public async Task UpdateStatusAsync(int id, FlightStatus status)
    {
        var flight = await _db.Flights.FindAsync(id)
            ?? throw new KeyNotFoundException($"Flight {id} bulunamadı.");
        flight.Status = status;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var flight = await _db.Flights.FindAsync(id)
            ?? throw new KeyNotFoundException($"Flight {id} bulunamadı.");
        flight.IsDeleted = true;
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int flightId)
    {
        var flight = await _db.Flights
            .Include(f => f.Aircraft).ThenInclude(a => a.Seats)
            .Include(f => f.Reservations.Where(r => r.Status == ReservationStatus.Active))
            .FirstOrDefaultAsync(f => f.Id == flightId)
            ?? throw new KeyNotFoundException($"Flight {flightId} bulunamadı.");

        var reservedSeatIds = flight.Reservations.Select(r => r.SeatId).ToHashSet();

        return flight.Aircraft.Seats
            .Where(s => !reservedSeatIds.Contains(s.Id))
            .OrderBy(s => s.RowNumber).ThenBy(s => s.ColumnLetter);
    }

    public async Task<bool> IsSeatAvailableAsync(int flightId, int seatId) =>
        !await _db.Reservations.AnyAsync(r =>
            r.FlightId == flightId &&
            r.SeatId == seatId &&
            r.Status == ReservationStatus.Active);
}
