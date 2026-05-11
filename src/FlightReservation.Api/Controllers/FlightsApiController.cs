using FlightReservation.Business.Interfaces;
using FlightReservation.Core.Enums;
using FlightReservation.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FlightsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IFlightService _flightService;

    public FlightsController(AppDbContext db, IFlightService flightService)
    {
        _db = db;
        _flightService = flightService;
    }

    /// <summary>Güzergah, tarih ve şehir bazlı sefer arama (LINQ)</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string origin, [FromQuery] string destination, [FromQuery] DateTime date)
    {
        var results = await _flightService.SearchAsync(origin, destination, date);

        var dto = results.Select(f => new
        {
            f.Id,
            f.FlightNumber,
            Origin = f.Route.OriginCity,
            OriginCode = f.Route.OriginCode,
            Destination = f.Route.DestinationCity,
            DestinationCode = f.Route.DestinationCode,
            f.DepartureUtc,
            f.ArrivalUtc,
            DurationMinutes = (int)f.Duration.TotalMinutes,
            Aircraft = f.Aircraft.Model,
            TotalSeats = f.Aircraft.TotalCapacity,
            AvailableSeats = f.Aircraft.TotalCapacity - f.Reservations.Count(r => r.Status == ReservationStatus.Active),
            f.Status,
            f.Gate,
            f.Terminal
        });

        return Ok(dto);
    }

    /// <summary>En çok rezervasyon yapılan güzergahlar (LINQ GroupBy)</summary>
    [HttpGet("popular-routes")]
    public async Task<IActionResult> PopularRoutes([FromQuery] int top = 5)
    {
        var result = await _db.Reservations
            .Where(r => r.Status == ReservationStatus.Active)
            .GroupBy(r => new
            {
                r.Flight.Route.OriginCity,
                r.Flight.Route.OriginCode,
                r.Flight.Route.DestinationCity,
                r.Flight.Route.DestinationCode
            })
            .Select(g => new
            {
                g.Key.OriginCity,
                g.Key.OriginCode,
                g.Key.DestinationCity,
                g.Key.DestinationCode,
                TotalReservations = g.Count()
            })
            .OrderByDescending(x => x.TotalReservations)
            .Take(top)
            .ToListAsync();

        return Ok(result);
    }

    /// <summary>Sefer koltuk haritası — dolu/boş (LINQ Join)</summary>
    [HttpGet("{id}/seatmap")]
    public async Task<IActionResult> SeatMap(int id)
    {
        var flight = await _db.Flights
            .Include(f => f.Aircraft).ThenInclude(a => a.Seats)
            .Include(f => f.Reservations.Where(r => r.Status == ReservationStatus.Active))
            .FirstOrDefaultAsync(f => f.Id == id);

        if (flight is null) return NotFound();

        var reservedSeatIds = flight.Reservations.Select(r => r.SeatId).ToHashSet();

        var seatMap = flight.Aircraft.Seats
            .OrderBy(s => s.RowNumber).ThenBy(s => s.ColumnLetter)
            .Select(s => new
            {
                s.Id,
                s.RowNumber,
                s.ColumnLetter,
                SeatNumber = s.SeatNumber,
                Class = s.SeatClass.ToString(),
                s.IsExitRow,
                s.IsWindowSeat,
                s.IsAisleSeat,
                IsOccupied = reservedSeatIds.Contains(s.Id)
            });

        return Ok(new
        {
            FlightId = id,
            FlightNumber = flight.FlightNumber,
            Aircraft = flight.Aircraft.Model,
            TotalRows = flight.Aircraft.TotalRows,
            SeatsPerRow = flight.Aircraft.SeatsPerRow,
            BusinessRows = flight.Aircraft.BusinessRowCount,
            Seats = seatMap
        });
    }

    /// <summary>Doluluk raporu (LINQ aggregation)</summary>
    [HttpGet("occupancy")]
    public async Task<IActionResult> OccupancyReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow;

        var report = await _db.Flights
            .Where(f => f.DepartureUtc >= fromDate && f.DepartureUtc <= toDate)
            .Select(f => new
            {
                f.FlightNumber,
                Origin = f.Route.OriginCity,
                Destination = f.Route.DestinationCity,
                f.DepartureUtc,
                TotalSeats = f.Aircraft.TotalRows * f.Aircraft.SeatsPerRow,
                OccupiedSeats = f.Reservations.Count(r => r.Status == ReservationStatus.Active),
                f.Status
            })
            .Select(x => new
            {
                x.FlightNumber,
                x.Origin,
                x.Destination,
                x.DepartureUtc,
                x.TotalSeats,
                x.OccupiedSeats,
                OccupancyRate = x.TotalSeats == 0 ? 0.0 : Math.Round((double)x.OccupiedSeats / x.TotalSeats * 100, 1),
                x.Status
            })
            .OrderBy(x => x.DepartureUtc)
            .ToListAsync();

        return Ok(report);
    }
}
