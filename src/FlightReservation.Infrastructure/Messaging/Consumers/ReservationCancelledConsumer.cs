using FlightReservation.Infrastructure.Email;
using FlightReservation.Messaging.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FlightReservation.Infrastructure.Messaging.Consumers;

public class ReservationCancelledConsumer : IConsumer<ReservationCancelledEvent>
{
    private readonly IEmailService _email;
    private readonly ILogger<ReservationCancelledConsumer> _logger;

    public ReservationCancelledConsumer(IEmailService email, ILogger<ReservationCancelledConsumer> logger)
    {
        _email = email;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReservationCancelledEvent> context)
    {
        var evt = context.Message;
        await _email.SendReservationCancellationAsync(
            evt.UserEmail, evt.UserFullName, evt.PnrCode, evt.CancelReason);

        _logger.LogInformation("Cancellation email sent: PNR={Pnr}", evt.PnrCode);
    }
}
