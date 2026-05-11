using FlightReservation.Infrastructure.Email;
using FlightReservation.Messaging.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FlightReservation.Infrastructure.Messaging.Consumers;

public class FlightCancelledConsumer : IConsumer<FlightCancelledEvent>
{
    private readonly IEmailService _email;
    private readonly ILogger<FlightCancelledConsumer> _logger;

    public FlightCancelledConsumer(IEmailService email, ILogger<FlightCancelledConsumer> logger)
    {
        _email = email;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FlightCancelledEvent> context)
    {
        var evt = context.Message;
        var flightInfo = $"Sefer: {evt.FlightNumber} | {evt.RouteInfo} | {evt.DepartureUtc:dd MMM yyyy HH:mm} UTC";

        var tasks = evt.AffectedPassengers.Select(p =>
            _email.SendFlightCancellationAsync(p.UserEmail, p.UserFullName, evt.FlightNumber, flightInfo));

        await Task.WhenAll(tasks);

        _logger.LogInformation("Flight cancellation emails sent: Flight={Flight}, Count={Count}",
            evt.FlightNumber, evt.AffectedPassengers.Count);
    }
}
