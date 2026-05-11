using FlightReservation.Core.Enums;
using FlightReservation.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlightReservation.Infrastructure.Hangfire.Jobs;

public class MarkCompletedFlightsJob
{
    private readonly AppDbContext _db;
    private readonly ILogger<MarkCompletedFlightsJob> _logger;

    public MarkCompletedFlightsJob(AppDbContext db, ILogger<MarkCompletedFlightsJob> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var now = DateTime.UtcNow;

        var flights = await _db.Flights
            .Where(f => f.Status == FlightStatus.Scheduled || f.Status == FlightStatus.Departed)
            .Where(f => f.ArrivalUtc < now)
            .ToListAsync();

        foreach (var flight in flights)
            flight.Status = FlightStatus.Completed;

        if (flights.Count > 0)
        {
            await _db.SaveChangesAsync();
            _logger.LogInformation("Marked {Count} flights as Completed.", flights.Count);
        }
    }
}
