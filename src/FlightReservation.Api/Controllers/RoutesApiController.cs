using FlightReservation.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RoutesController : ControllerBase
{
    private readonly AppDbContext _db;

    public RoutesController(AppDbContext db) => _db = db;

    /// <summary>Aktif güzergahlar (LINQ)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Routes
            .Where(r => r.IsActive)
            .OrderBy(r => r.OriginCity)
            .Select(r => new
            {
                r.Id,
                r.OriginCity,
                r.OriginCode,
                r.DestinationCity,
                r.DestinationCode,
                r.DistanceKm,
                r.EstimatedDurationMinutes
            })
            .ToListAsync());

    /// <summary>Güzergah istatistikleri (LINQ)</summary>
    [HttpGet("stats")]
    public async Task<IActionResult> Stats() =>
        Ok(await _db.Routes
            .Where(r => r.IsActive)
            .Select(r => new
            {
                r.OriginCity,
                r.DestinationCity,
                TotalFlights = r.Flights.Count,
                UpcomingFlights = r.Flights.Count(f => f.DepartureUtc > DateTime.UtcNow),
                TotalPassengers = r.Flights.SelectMany(f => f.Reservations).Count()
            })
            .OrderByDescending(x => x.TotalPassengers)
            .ToListAsync());
}
