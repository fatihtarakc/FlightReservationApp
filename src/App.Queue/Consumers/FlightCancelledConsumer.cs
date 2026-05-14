namespace App.Queue.Consumers
{
    public class FlightCancelledConsumer : IConsumer<FlightCancelledEvent>
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<FlightCancelledConsumer> _logger;

        public FlightCancelledConsumer(
            IEmailSenderService emailSenderService,
            ISmsSenderService smsSenderService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<FlightCancelledConsumer> logger)
        {
            _emailSenderService = emailSenderService;
            _smsSenderService = smsSenderService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<FlightCancelledEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Processing FlightCancelledEvent for FlightId: {FlightId}, AffectedPassengers: {Count}",
                message.FlightId, message.AffectedPassengers.Count());

            var passengerTasks = message.AffectedPassengers
                .Select(passenger => NotifyPassengerAsync(message, passenger));

            await Task.WhenAll(passengerTasks);
            _logger.LogInformation("{Message} FlightId: {FlightId}", _localizer[Messages.MassTransit_Consume_Was_Successful].Value, message.FlightId);
        }

        private async Task NotifyPassengerAsync(FlightCancelledEvent message, AffectedPassenger passenger)
        {
            var tasks = new List<Task>();

            if (passenger.PreferredChannel.HasFlag(NotificationChannel.Email))
                tasks.Add(SendCancellationEmailAsync(message, passenger));

            if (passenger.PreferredChannel.HasFlag(NotificationChannel.Sms))
                tasks.Add(SendCancellationSmsAsync(message, passenger));

            if (passenger.PreferredChannel.HasFlag(NotificationChannel.WhatsApp))
                tasks.Add(SendCancellationWhatsAppAsync(message, passenger));

            await Task.WhenAll(tasks);
        }

        private async Task SendCancellationEmailAsync(FlightCancelledEvent message, AffectedPassenger passenger)
        {
            var reasonRow = !string.IsNullOrWhiteSpace(message.CancellationReason)
                ? $"<tr><td style=\"color:#777;padding:6px 0;\">Cancellation Reason</td><td style=\"color:#222;font-weight:bold;text-align:right;\">{message.CancellationReason}</td></tr>"
                : string.Empty;

            var body = $@"<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""></head>
<body style=""font-family:Arial,sans-serif;background-color:#f4f4f4;margin:0;padding:20px;"">
  <div style=""max-width:600px;margin:0 auto;background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,.1);"">
    <div style=""background-color:#e74c3c;padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; FlightReservation</h1>
      <p style=""color:#fadbd8;margin:8px 0 0;"">Important: Flight Cancellation Notice</p>
    </div>
    <div style=""padding:30px;"">
      <h2 style=""color:#e74c3c;"">Your flight has been cancelled</h2>
      <p style=""color:#555;line-height:1.7;"">Dear {passenger.Name}, we regret to inform you that your flight has been cancelled.</p>
      <div style=""margin:20px 0;padding:20px;background-color:#fef9f9;border-radius:6px;border:1px solid #f5c6c2;"">
        <h3 style=""color:#e74c3c;margin:0 0 15px;"">Flight Details</h3>
        <table style=""width:100%;border-collapse:collapse;"">
          <tr><td style=""color:#777;padding:6px 0;"">PNR Number</td><td style=""color:#222;font-weight:bold;text-align:right;"">{passenger.PnrNumber}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">Flight</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.FlightNumber}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">Route</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureCity} &#8594; {message.ArrivalCity}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">Scheduled Departure</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureDateTime:dd MMM yyyy HH:mm}</td></tr>
          {reasonRow}
        </table>
      </div>
      <div style=""padding:16px;background-color:#fff3cd;border-radius:4px;border-left:4px solid #ffc107;"">
        <p style=""margin:0;color:#856404;"">Our team will contact you shortly to arrange an alternative flight or process your refund. We sincerely apologize for any inconvenience caused.</p>
      </div>
    </div>
    <div style=""background-color:#f8f8f8;padding:20px;text-align:center;border-top:1px solid #eee;"">
      <p style=""color:#999;font-size:12px;margin:0;"">&#169; 2026 FlightReservation. All rights reserved.</p>
    </div>
  </div>
</body>
</html>";

            await _emailSenderService.SendAsync(new EmailDto
            {
                To = passenger.Email,
                Subject = $"{_localizer[Messages.EmailSubject_FlightCancelled]} - {message.FlightNumber}",
                Body = body,
                IsHtml = true
            });
        }

        private async Task SendCancellationSmsAsync(FlightCancelledEvent message, AffectedPassenger passenger)
        {
            var reason = !string.IsNullOrWhiteSpace(message.CancellationReason) ? $" Reason: {message.CancellationReason}." : string.Empty;
            var smsBody = $"IMPORTANT: Flight {message.FlightNumber} ({message.DepartureCity}-{message.ArrivalCity}) on {message.DepartureDateTime:dd MMM yyyy} has been cancelled. PNR: {passenger.PnrNumber}.{reason} We will contact you for rebooking.";
            await _smsSenderService.SendSmsAsync(passenger.PhoneNumber, smsBody);
        }

        private async Task SendCancellationWhatsAppAsync(FlightCancelledEvent message, AffectedPassenger passenger)
        {
            var reason = !string.IsNullOrWhiteSpace(message.CancellationReason) ? $"\n*Reason:* {message.CancellationReason}" : string.Empty;
            var whatsAppBody = $"⚠️ *Flight Cancellation Notice*\n\n" +
                               $"Dear {passenger.Name},\n\n" +
                               $"*Flight:* {message.FlightNumber}\n" +
                               $"*Route:* {message.DepartureCity} → {message.ArrivalCity}\n" +
                               $"*Scheduled:* {message.DepartureDateTime:dd MMM yyyy HH:mm}\n" +
                               $"*Your PNR:* {passenger.PnrNumber}{reason}\n\n" +
                               $"We will contact you shortly to arrange an alternative flight or refund. We apologize for the inconvenience.";
            await _smsSenderService.SendWhatsAppAsync(passenger.PhoneNumber, whatsAppBody);
        }
    }
}

