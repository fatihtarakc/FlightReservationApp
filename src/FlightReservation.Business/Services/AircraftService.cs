using FlightReservation.Business.Interfaces;
using FlightReservation.Core.Entities;
using FlightReservation.Core.Enums;
using FlightReservation.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Business.Services;

public class AircraftService : IAircraftService
{
    private readonly AppDbContext _db;

    public AircraftService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Aircraft>> GetAllActiveAsync() =>
        await _db.Aircrafts.Where(a => a.IsActive).OrderBy(a => a.Model).ToListAsync();

    public async Task<Aircraft?> GetByIdWithSeatsAsync(int id) =>
        await _db.Aircrafts
            .Include(a => a.Seats.OrderBy(s => s.RowNumber).ThenBy(s => s.ColumnLetter))
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Aircraft> CreateAsync(Aircraft aircraft)
    {
        aircraft.Seats = GenerateSeats(aircraft.TotalRows, aircraft.SeatsPerRow, aircraft.BusinessRowCount);
        _db.Aircrafts.Add(aircraft);
        await _db.SaveChangesAsync();
        return aircraft;
    }

    public async Task<Aircraft> UpdateAsync(Aircraft aircraft)
    {
        _db.Aircrafts.Update(aircraft);
        await _db.SaveChangesAsync();
        return aircraft;
    }

    public async Task DeleteAsync(int id)
    {
        var aircraft = await _db.Aircrafts.FindAsync(id)
            ?? throw new KeyNotFoundException($"Aircraft {id} bulunamadı.");
        aircraft.IsActive = false;
        aircraft.IsDeleted = true;
        await _db.SaveChangesAsync();
    }

    private static List<Seat> GenerateSeats(int totalRows, int seatsPerRow, int businessRows)
    {
        var columns = seatsPerRow switch
        {
            6 => new[] { "A", "B", "C", "D", "E", "F" },
            9 => new[] { "A", "B", "C", "D", "E", "F", "G", "H", "J" },
            _ => Enumerable.Range(0, seatsPerRow).Select(i => ((char)('A' + i)).ToString()).ToArray()
        };

        var seats = new List<Seat>();
        for (int row = 1; row <= totalRows; row++)
        {
            var seatClass = row <= businessRows ? SeatClass.Business : SeatClass.Economy;
            foreach (var col in columns)
            {
                seats.Add(new Seat
                {
                    RowNumber = row,
                    ColumnLetter = col,
                    SeatClass = seatClass,
                    IsExitRow = row == businessRows + 1 || row == totalRows - 2
                });
            }
        }
        return seats;
    }
}
