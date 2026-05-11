using FlightReservation.Core.Enums;
using FlightReservation.Data.Context;
using FlightReservation.Infrastructure.Cache;
using FlightReservation.Messaging.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlightReservation.Infrastructure.Hangfire.Jobs;

public class FlightReminderJob
{
    private readonly AppDbContext _db;
    private readonly IPublishEndpoint _publish;
    private readonly ICacheService _cache;
    private readonly ILogger<FlightReminderJob> _logger;

    public FlightReminderJob(AppDbContext db, IPublishEndpoint publish, ICacheService cache, ILogger<FlightReminderJob> logger)
    {
        _db = db;
        _publish = publish;
        _cache = cache;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var windowStart = DateTime.UtcNow.AddHours(23);
        var windowEnd = DateTime.UtcNow.AddHours(25);

        var reservations = await _db.Reservations
            .Include(r => r.Flight).ThenInclude(f => f.Route)
            .Include(r => r.Seat)
            .Include(r => r.User)
            .Where(r =>
                r.Status == ReservationStatus.Active &&
                r.Flight.Status == FlightStatus.Scheduled &&
                r.Flight.DepartureUtc >= windowStart &&
                r.Flight.DepartureUtc <= windowEnd)
            .ToListAsync();

        foreach (var r in reservations)
        {
            var dedupKey = $"reminder:sent:{r.Id}";
            if (await _cache.ExistsAsync(dedupKey)) continue;

            await _publish.Publish(new FlightReminderEvent(
                r.User.Email!,
                r.User.FullName,
                r.PnrCode,
                r.Flight.FlightNumber,
                $"{r.Flight.Route.OriginCity} → {r.Flight.Route.DestinationCity}",
                r.Seat.SeatNumber,
                r.Flight.DepartureUtc
            ));

            await _cache.SetAsync(dedupKey, "1", TimeSpan.FromHours(30));
        }

        _logger.LogInformation("Flight reminders published: {Count}", reservations.Count);
    }
}
