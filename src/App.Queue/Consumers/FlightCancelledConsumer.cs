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
            CultureInfo.CurrentUICulture = new CultureInfo(message.Language);

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
            var dear = _localizer[Messages.Email_Dear].Value;
            var footer = _localizer[Messages.Email_Footer].Value;

            var reasonRow = !string.IsNullOrWhiteSpace(message.CancellationReason)
                ? $"<tr><td class=\"lbl\" style=\"color:#777;padding:6px 0;\">{_localizer[Messages.Email_Label_CancellationReason]}</td><td class=\"val\" style=\"color:#222;font-weight:bold;text-align:right;\">{message.CancellationReason}</td></tr>"
                : string.Empty;

            var body = $@"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <style>
    @media screen and (max-width:600px) {{
      .email-body {{ padding:8px !important; }}
      .email-wrap {{ border-radius:0 !important; }}
      .email-content {{ padding:20px 16px !important; }}
      .info-table .lbl,
      .info-table .val {{ display:block !important; text-align:left !important; padding:2px 0 !important; }}
      .info-table .lbl {{ font-size:11px; color:#888 !important; }}
      .info-table .val {{ padding-bottom:8px !important; }}
    }}
  </style>
</head>
<body class=""email-body"" style=""font-family:Arial,sans-serif;background-color:#f4f4f4;margin:0;padding:20px;"">
  <div class=""email-wrap"" style=""max-width:600px;margin:0 auto;background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,.1);"">
    <div style=""background-color:#e74c3c;padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; Flight Reservation</h1>
      <p style=""color:#fadbd8;margin:8px 0 0;"">{_localizer[Messages.Email_FlightCancelled_HeaderSubtitle]}</p>
    </div>
    <div class=""email-content"" style=""padding:30px;"">
      <h2 style=""color:#e74c3c;"">{_localizer[Messages.Email_FlightCancelled_Heading]}</h2>
      <p style=""color:#555;line-height:1.7;"">{dear} {passenger.Name}, {_localizer[Messages.Email_FlightCancelled_Intro]}</p>
      <div style=""margin:20px 0;padding:20px;background-color:#fef9f9;border-radius:6px;border:1px solid #f5c6c2;"">
        <h3 style=""color:#e74c3c;margin:0 0 15px;"">{_localizer[Messages.Email_FlightCancelled_SectionTitle]}</h3>
        <table class=""info-table"" style=""width:100%;border-collapse:collapse;"">
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_PnrNumber]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{passenger.PnrNumber}</td></tr>
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Flight]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.FlightNumber}</td></tr>
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Route]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureCity} &#8594; {message.ArrivalCity}</td></tr>
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_ScheduledDeparture]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureDateTime:dd MMM yyyy HH:mm}</td></tr>
          {reasonRow}
        </table>
      </div>
      <div style=""padding:16px;background-color:#fff3cd;border-radius:4px;border-left:4px solid #ffc107;"">
        <p style=""margin:0;color:#856404;"">{_localizer[Messages.Email_FlightCancelled_Warning]}</p>
      </div>
    </div>
    <div style=""background-color:#f8f8f8;padding:20px;text-align:center;border-top:1px solid #eee;"">
      <p style=""color:#999;font-size:12px;margin:0;"">{footer}</p>
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
            var reason = !string.IsNullOrWhiteSpace(message.CancellationReason) ? $" {message.CancellationReason}." : string.Empty;
            var smsBody = string.Format(_localizer[Messages.Sms_FlightCancelled].Value,
                message.FlightNumber, message.DepartureCity, message.ArrivalCity,
                message.DepartureDateTime.ToString("dd MMM yyyy"), passenger.PnrNumber, reason);
            await _smsSenderService.SendSmsAsync(passenger.PhoneNumber, smsBody);
        }

        private async Task SendCancellationWhatsAppAsync(FlightCancelledEvent message, AffectedPassenger passenger)
        {
            var reason = !string.IsNullOrWhiteSpace(message.CancellationReason)
                ? $"\n*{_localizer[Messages.Email_Label_CancellationReason]}:* {message.CancellationReason}"
                : string.Empty;
            var whatsAppBody = string.Format(_localizer[Messages.WhatsApp_FlightCancelled].Value,
                message.FlightNumber, message.DepartureCity, message.ArrivalCity,
                message.DepartureDateTime.ToString("dd MMM yyyy HH:mm"), passenger.PnrNumber, reason)
                .Replace("\\n", "\n");
            await _smsSenderService.SendWhatsAppAsync(passenger.PhoneNumber, whatsAppBody);
        }
    }
}
