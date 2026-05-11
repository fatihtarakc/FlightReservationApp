using FlightReservation.Infrastructure.Email;
using FlightReservation.Messaging.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FlightReservation.Infrastructure.Messaging.Consumers;

public class ReservationCreatedConsumer : IConsumer<ReservationCreatedEvent>
{
    private readonly IEmailService _email;
    private readonly ILogger<ReservationCreatedConsumer> _logger;

    public ReservationCreatedConsumer(IEmailService email, ILogger<ReservationCreatedConsumer> logger)
    {
        _email = email;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReservationCreatedEvent> context)
    {
        var evt = context.Message;

        var flightInfo = $"""
            Sefer: {evt.FlightNumber}<br>
            Güzergah: {evt.RouteInfo}<br>
            Kalkış: {evt.DepartureUtc:dd MMM yyyy HH:mm} UTC<br>
            Koltuk: {evt.SeatNumber}
            """;

        await _email.SendReservationConfirmationAsync(
            evt.UserEmail, evt.UserFullName, evt.PnrCode, flightInfo);

        _logger.LogInformation("Reservation confirmation email sent: PNR={Pnr} Email={Email}",
            evt.PnrCode, evt.UserEmail);
    }
}
