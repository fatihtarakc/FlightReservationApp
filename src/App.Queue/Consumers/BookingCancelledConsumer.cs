namespace App.Queue.Consumers
{
    public class BookingCancelledConsumer : IConsumer<BookingCancelledEvent>
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<BookingCancelledConsumer> _logger;

        public BookingCancelledConsumer(
            IEmailSenderService emailSenderService,
            ISmsSenderService smsSenderService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<BookingCancelledConsumer> logger)
        {
            _emailSenderService = emailSenderService;
            _smsSenderService = smsSenderService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BookingCancelledEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Processing BookingCancelledEvent for BookingId: {BookingId}", message.BookingId);

            var tasks = new List<Task>();

            if (message.PreferredChannel.HasFlag(NotificationChannel.Email))
                tasks.Add(SendCancellationEmailAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.Sms))
                tasks.Add(SendCancellationSmsAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.WhatsApp))
                tasks.Add(SendCancellationWhatsAppAsync(message));

            await Task.WhenAll(tasks);
            _logger.LogInformation("{Message} BookingId: {BookingId}", _localizer[Messages.MassTransit_Consume_Was_Successful].Value, message.BookingId);
        }

        private async Task SendCancellationEmailAsync(BookingCancelledEvent message)
        {
            var reasonRow = !string.IsNullOrWhiteSpace(message.CancellationReason)
                ? $"<tr><td style=\"color:#777;padding:6px 0;\">Cancellation Reason</td><td style=\"color:#222;font-weight:bold;text-align:right;\">{message.CancellationReason}</td></tr>"
                : string.Empty;

            var body = $@"<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""></head>
<body style=""font-family:Arial,sans-serif;background-color:#f4f4f4;margin:0;padding:20px;"">
  <div style=""max-width:600px;margin:0 auto;background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,.1);"">
    <div style=""background-color:#c0392b;padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; FlightReservation</h1>
      <p style=""color:#f5a9a0;margin:8px 0 0;"">Booking Cancellation</p>
    </div>
    <div style=""padding:30px;"">
      <h2 style=""color:#c0392b;"">Your booking has been cancelled</h2>
      <p style=""color:#555;line-height:1.7;"">Dear {message.PassengerName}, your reservation has been cancelled.</p>
      <div style=""margin:20px 0;padding:20px;background-color:#fff5f5;border-radius:6px;border:1px solid #ffd0cc;"">
        <h3 style=""color:#c0392b;margin:0 0 15px;"">Cancellation Details</h3>
        <table style=""width:100%;border-collapse:collapse;"">
          <tr><td style=""color:#777;padding:6px 0;"">PNR Number</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.PnrNumber}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">Flight</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.FlightNumber}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">Route</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureCity} &#8594; {message.ArrivalCity}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">Departure</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureDateTime:dd MMM yyyy HH:mm}</td></tr>
          {reasonRow}
        </table>
      </div>
      <p style=""color:#555;font-size:13px;"">If you have any questions or need assistance with rebooking, please contact our support team.</p>
    </div>
    <div style=""background-color:#f8f8f8;padding:20px;text-align:center;border-top:1px solid #eee;"">
      <p style=""color:#999;font-size:12px;margin:0;"">&#169; 2026 FlightReservation. All rights reserved.</p>
    </div>
  </div>
</body>
</html>";

            await _emailSenderService.SendAsync(new EmailDto
            {
                To = message.Email,
                Subject = $"{_localizer[Messages.EmailSubject_BookingCancellation]} - {message.PnrNumber}",
                Body = body,
                IsHtml = true
            });
        }

        private async Task SendCancellationSmsAsync(BookingCancelledEvent message)
        {
            var reason = !string.IsNullOrWhiteSpace(message.CancellationReason) ? $" Reason: {message.CancellationReason}." : string.Empty;
            var smsBody = $"Booking cancelled. PNR: {message.PnrNumber} | Flight: {message.FlightNumber} | {message.DepartureCity}-{message.ArrivalCity} | {message.DepartureDateTime:dd MMM yyyy HH:mm}.{reason}";
            await _smsSenderService.SendSmsAsync(message.PhoneNumber, smsBody);
        }

        private async Task SendCancellationWhatsAppAsync(BookingCancelledEvent message)
        {
            var reason = !string.IsNullOrWhiteSpace(message.CancellationReason) ? $"\n*Reason:* {message.CancellationReason}" : string.Empty;
            var whatsAppBody = $"✈ *Booking Cancelled*\n\n" +
                               $"*PNR:* {message.PnrNumber}\n" +
                               $"*Flight:* {message.FlightNumber}\n" +
                               $"*Route:* {message.DepartureCity} → {message.ArrivalCity}\n" +
                               $"*Departure:* {message.DepartureDateTime:dd MMM yyyy HH:mm}{reason}\n\n" +
                               $"Contact support if you need assistance with rebooking.";
            await _smsSenderService.SendWhatsAppAsync(message.PhoneNumber, whatsAppBody);
        }
    }
}

