namespace App.Queue.Consumers
{
    public class BookingConfirmedConsumer : IConsumer<BookingConfirmedEvent>
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<BookingConfirmedConsumer> _logger;

        public BookingConfirmedConsumer(
            IEmailSenderService emailSenderService,
            ISmsSenderService smsSenderService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<BookingConfirmedConsumer> logger)
        {
            _emailSenderService = emailSenderService;
            _smsSenderService = smsSenderService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BookingConfirmedEvent> context)
        {
            var message = context.Message;
            CultureInfo.CurrentUICulture = new CultureInfo(message.Language);

            _logger.LogInformation("Processing BookingConfirmedEvent for BookingId: {BookingId}", message.BookingId);

            var tasks = new List<Task>();

            if (message.PreferredChannel.HasFlag(NotificationChannel.Email))
                tasks.Add(SendConfirmationEmailAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.Sms))
                tasks.Add(SendConfirmationSmsAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.WhatsApp))
                tasks.Add(SendConfirmationWhatsAppAsync(message));

            await Task.WhenAll(tasks);
            _logger.LogInformation("{Message} BookingId: {BookingId}", _localizer[Messages.MassTransit_Consume_Was_Successful].Value, message.BookingId);
        }

        private async Task SendConfirmationEmailAsync(BookingConfirmedEvent message)
        {
            var dear = _localizer[Messages.Email_Dear].Value;
            var footer = _localizer[Messages.Email_Footer].Value;

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
    <div style=""background-color:#003580;padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; Flight Reservation</h1>
      <p style=""color:#a8c4e0;margin:8px 0 0;"">{_localizer[Messages.Email_BookingConfirmed_HeaderSubtitle]}</p>
    </div>
    <div class=""email-content"" style=""padding:30px;"">
      <h2 style=""color:#003580;"">{_localizer[Messages.Email_BookingConfirmed_Heading]}</h2>
      <p style=""color:#555;line-height:1.7;"">{dear} {message.PassengerName}, {_localizer[Messages.Email_BookingConfirmed_Intro]}</p>
      <div style=""margin:20px 0;padding:20px;background-color:#f0f5ff;border-radius:6px;border:1px solid #d0e4ff;"">
        <h3 style=""color:#003580;margin:0 0 15px;"">{_localizer[Messages.Email_BookingConfirmed_SectionTitle]}</h3>
        <table class=""info-table"" style=""width:100%;border-collapse:collapse;"">
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_PnrNumber]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.PnrNumber}</td></tr>
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Flight]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.FlightNumber}</td></tr>
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Route]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureCity} &#8594; {message.ArrivalCity}</td></tr>
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Departure]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureDateTime:dd MMM yyyy HH:mm}</td></tr>
          <tr><td class=""lbl"" style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Seat]}</td><td class=""val"" style=""color:#222;font-weight:bold;text-align:right;"">{message.SeatNumber} ({message.SeatClass})</td></tr>
          <tr style=""border-top:1px solid #d0e4ff;""><td class=""lbl"" style=""color:#003580;padding:10px 0 0;font-weight:bold;"">{_localizer[Messages.Email_Label_TotalAmount]}</td><td class=""val"" style=""color:#003580;font-weight:bold;text-align:right;padding-top:10px;font-size:18px;"">{message.TotalPrice:C2}</td></tr>
        </table>
      </div>
      <p style=""color:#555;font-size:13px;"">{_localizer[Messages.Email_BookingConfirmed_Note]}</p>
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
                Subject = $"{_localizer[Messages.EmailSubject_BookingConfirmation]} - {message.PnrNumber}",
                Body = body,
                IsHtml = true
            });
        }

        private async Task SendConfirmationSmsAsync(BookingConfirmedEvent message)
        {
            var smsBody = string.Format(_localizer[Messages.Sms_BookingConfirmed].Value,
                message.PnrNumber, message.FlightNumber,
                message.DepartureCity, message.ArrivalCity,
                message.DepartureDateTime.ToString("dd MMM yyyy HH:mm"),
                message.SeatNumber, message.TotalPrice.ToString("C2"));
            await _smsSenderService.SendSmsAsync(message.PhoneNumber, smsBody);
        }

        private async Task SendConfirmationWhatsAppAsync(BookingConfirmedEvent message)
        {
            var whatsAppBody = string.Format(_localizer[Messages.WhatsApp_BookingConfirmed].Value,
                message.PnrNumber, message.FlightNumber,
                message.DepartureCity, message.ArrivalCity,
                message.DepartureDateTime.ToString("dd MMM yyyy HH:mm"),
                message.SeatNumber, message.SeatClass, message.TotalPrice.ToString("C2"))
                .Replace("\\n", "\n");
            await _smsSenderService.SendWhatsAppAsync(message.PhoneNumber, whatsAppBody);
        }
    }
}
