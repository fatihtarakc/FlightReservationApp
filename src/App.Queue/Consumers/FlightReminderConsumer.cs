namespace App.Queue.Consumers
{
    public class FlightReminderConsumer : IConsumer<FlightReminderEvent>
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<FlightReminderConsumer> _logger;

        public FlightReminderConsumer(
            IEmailSenderService emailSenderService,
            ISmsSenderService smsSenderService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<FlightReminderConsumer> logger)
        {
            _emailSenderService = emailSenderService;
            _smsSenderService = smsSenderService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<FlightReminderEvent> context)
        {
            var message = context.Message;
            CultureInfo.CurrentUICulture = new CultureInfo(message.Language);

            var reminderType = message.Is7DayReminder ? "7-Day" : "24-Hour";
            _logger.LogInformation("Processing FlightReminderEvent ({ReminderType}) for BookingId: {BookingId}", reminderType, message.BookingId);

            var tasks = new List<Task>();

            if (message.PreferredChannel.HasFlag(NotificationChannel.Email))
                tasks.Add(SendReminderEmailAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.Sms))
                tasks.Add(SendReminderSmsAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.WhatsApp))
                tasks.Add(SendReminderWhatsAppAsync(message));

            await Task.WhenAll(tasks);
            _logger.LogInformation("{Message} BookingId: {BookingId} ReminderType: {ReminderType}",
                _localizer[Messages.MassTransit_Consume_Was_Successful].Value, message.BookingId, reminderType);
        }

        private async Task SendReminderEmailAsync(FlightReminderEvent message)
        {
            var footer = _localizer[Messages.Email_Footer].Value;
            var (subjectKey, headerColor, reminderBadge, reminderDesc) = message.Is7DayReminder
                ? (Messages.EmailSubject_FlightReminder_7Days, "#2980b9",
                   _localizer[Messages.Email_FlightReminder_7Days_Badge].Value,
                   _localizer[Messages.Email_FlightReminder_7Days_Desc].Value)
                : (Messages.EmailSubject_FlightReminder_24Hours, "#e67e22",
                   _localizer[Messages.Email_FlightReminder_24Hours_Badge].Value,
                   _localizer[Messages.Email_FlightReminder_24Hours_Desc].Value);

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
    <div style=""background-color:{headerColor};padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; Flight Reservation</h1>
      <p style=""color:rgba(255,255,255,.8);margin:8px 0 0;"">{_localizer[Messages.Email_FlightReminder_HeaderSubtitle]}</p>
    </div>
    <div class=""email-content"" style=""padding:30px;"">
      <div style=""text-align:center;margin-bottom:20px;"">
        <span style=""display:inline-block;background-color:{headerColor};color:#fff;padding:8px 20px;border-radius:20px;font-weight:bold;font-size:14px;"">{reminderBadge}</span>
      </div>
      <h2 style=""color:#333;"">{_localizer[Messages.Email_Dear]} {message.PassengerName},</h2>
      <p style=""color:#555;line-height:1.7;"">{reminderDesc}</p>
      <div style=""margin:20px 0;padding:20px;background-color:#f8f9ff;border-radius:6px;border:1px solid #d0deff;"">
        <h3 style=""color:#003580;margin:0 0 15px;"">{_localizer[Messages.Email_FlightReminder_SectionTitle]}</h3>
        <table class=""info-table"" style=""width:100%;border-collapse:collapse;"">
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_PnrNumber]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.PnrNumber}</td></tr>
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Flight]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.FlightNumber}</td></tr>
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Route]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureAirport} &#8594; {message.ArrivalAirport}</td></tr>
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Departure]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;font-size:16px;"">{message.DepartureDateTime:dd MMM yyyy HH:mm}</td></tr>
        </table>
      </div>
      <p style=""color:#555;font-size:13px;"">{_localizer[Messages.Email_FlightReminder_Closing]} &#127758;</p>
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
                Subject = $"{_localizer[subjectKey]} - {message.FlightNumber}",
                Body = body,
                IsHtml = true
            });
        }

        private async Task SendReminderSmsAsync(FlightReminderEvent message)
        {
            var isEn = CultureInfo.CurrentUICulture.Name.StartsWith("en");
            var timeLabel = message.Is7DayReminder
                ? (isEn ? "7 days" : "7 gün")
                : (isEn ? "24 hours" : "24 saat");
            var smsBody = string.Format(_localizer[Messages.Sms_FlightReminder].Value,
                message.FlightNumber, message.DepartureAirport, message.ArrivalAirport,
                timeLabel, message.DepartureDateTime.ToString("dd MMM yyyy HH:mm"), message.PnrNumber);
            await _smsSenderService.SendSmsAsync(message.PhoneNumber, smsBody);
        }

        private async Task SendReminderWhatsAppAsync(FlightReminderEvent message)
        {
            var isEn = CultureInfo.CurrentUICulture.Name.StartsWith("en");
            var timeLabel = message.Is7DayReminder
                ? (isEn ? "7 days" : "7 gün")
                : (isEn ? "24 hours" : "24 saat");
            var whatsAppBody = string.Format(_localizer[Messages.WhatsApp_FlightReminder].Value,
                timeLabel, message.FlightNumber, message.DepartureAirport, message.ArrivalAirport,
                message.DepartureDateTime.ToString("dd MMM yyyy HH:mm"), message.PnrNumber)
                .Replace("\\n", "\n");
            await _smsSenderService.SendWhatsAppAsync(message.PhoneNumber, whatsAppBody);
        }
    }
}
