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
            CultureInfo.CurrentUICulture = new CultureInfo(message.Language);

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
            var dear = _localizer[Messages.Email_Dear].Value;
            var footer = _localizer[Messages.Email_Footer].Value;

            var reasonRow = !string.IsNullOrWhiteSpace(message.CancellationReason)
                ? $"<tr><td style=\"color:#777;padding:6px 0;\">{_localizer[Messages.Email_Label_CancellationReason]}</td><td style=\"color:#222;font-weight:bold;text-align:right;\">{message.CancellationReason}</td></tr>"
                : string.Empty;

            var body = $@"<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""></head>
<body style=""font-family:Arial,sans-serif;background-color:#f4f4f4;margin:0;padding:20px;"">
  <div style=""max-width:600px;margin:0 auto;background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,.1);"">
    <div style=""background-color:#c0392b;padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; Flight Reservation</h1>
      <p style=""color:#f5a9a0;margin:8px 0 0;"">{_localizer[Messages.Email_BookingCancelled_HeaderSubtitle]}</p>
    </div>
    <div style=""padding:30px;"">
      <h2 style=""color:#c0392b;"">{_localizer[Messages.Email_BookingCancelled_Heading]}</h2>
      <p style=""color:#555;line-height:1.7;"">{dear} {message.PassengerName}, {_localizer[Messages.Email_BookingCancelled_Intro]}</p>
      <div style=""margin:20px 0;padding:20px;background-color:#fff5f5;border-radius:6px;border:1px solid #ffd0cc;"">
        <h3 style=""color:#c0392b;margin:0 0 15px;"">{_localizer[Messages.Email_BookingCancelled_SectionTitle]}</h3>
        <table style=""width:100%;border-collapse:collapse;"">
          <tr><td style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_PnrNumber]}</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.PnrNumber}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Flight]}</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.FlightNumber}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Route]}</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureCity} &#8594; {message.ArrivalCity}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Departure]}</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureDateTime:dd MMM yyyy HH:mm}</td></tr>
          {reasonRow}
        </table>
      </div>
      <p style=""color:#555;font-size:13px;"">{_localizer[Messages.Email_BookingCancelled_ContactNote]}</p>
    </div>
    <div style=""background-color:#f8f8f8;padding:20px;text-align:center;border-top:1px solid #eee;"">
      <p style=""color:#999;font-size:12px;margin:0;"">{footer}</p>
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
            var reason = !string.IsNullOrWhiteSpace(message.CancellationReason) ? $" {message.CancellationReason}." : string.Empty;
            var smsBody = string.Format(_localizer[Messages.Sms_BookingCancelled].Value,
                message.PnrNumber, message.FlightNumber,
                message.DepartureCity, message.ArrivalCity,
                message.DepartureDateTime.ToString("dd MMM yyyy HH:mm"), reason);
            await _smsSenderService.SendSmsAsync(message.PhoneNumber, smsBody);
        }

        private async Task SendCancellationWhatsAppAsync(BookingCancelledEvent message)
        {
            var reason = !string.IsNullOrWhiteSpace(message.CancellationReason) ? $"\n*Reason:* {message.CancellationReason}" : string.Empty;
            var whatsAppBody = string.Format(_localizer[Messages.WhatsApp_BookingCancelled].Value,
                message.PnrNumber, message.FlightNumber,
                message.DepartureCity, message.ArrivalCity,
                message.DepartureDateTime.ToString("dd MMM yyyy HH:mm"), reason)
                .Replace("\\n", "\n");
            await _smsSenderService.SendWhatsAppAsync(message.PhoneNumber, whatsAppBody);
        }
    }
}
