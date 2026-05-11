using FlightReservation.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlightReservation.Infrastructure.Hangfire.Jobs;

public class CleanupAuditLogsJob
{
    private readonly AppDbContext _db;
    private readonly ILogger<CleanupAuditLogsJob> _logger;

    public CleanupAuditLogsJob(AppDbContext db, ILogger<CleanupAuditLogsJob> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-90);

        var deleted = await _db.AuditLogs
            .Where(a => a.Timestamp < cutoff)
            .ExecuteDeleteAsync();

        _logger.LogInformation("AuditLog cleanup: {Count} records deleted (older than {Cutoff:yyyy-MM-dd}).", deleted, cutoff);
    }
}
