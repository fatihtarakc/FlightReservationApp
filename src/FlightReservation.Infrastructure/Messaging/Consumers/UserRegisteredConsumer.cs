using FlightReservation.Infrastructure.Email;
using FlightReservation.Messaging.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FlightReservation.Infrastructure.Messaging.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IEmailService _email;
    private readonly ILogger<UserRegisteredConsumer> _logger;

    public UserRegisteredConsumer(IEmailService email, ILogger<UserRegisteredConsumer> logger)
    {
        _email = email;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var evt = context.Message;
        await _email.SendWelcomeEmailAsync(evt.Email, evt.FullName, evt.ConfirmationLink);
        _logger.LogInformation("Welcome email sent: {Email}", evt.Email);
    }
}
