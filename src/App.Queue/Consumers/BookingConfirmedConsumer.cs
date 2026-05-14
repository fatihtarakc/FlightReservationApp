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
            var body = $@"<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""></head>
<body style=""font-family:Arial,sans-serif;background-color:#f4f4f4;margin:0;padding:20px;"">
  <div style=""max-width:600px;margin:0 auto;background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,.1);"">
    <div style=""background-color:#003580;padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; FlightReservation</h1>
      <p style=""color:#a8c4e0;margin:8px 0 0;"">Booking Confirmation</p>
    </div>
    <div style=""padding:30px;"">
      <h2 style=""color:#003580;"">Your booking is confirmed!</h2>
      <p style=""color:#555;line-height:1.7;"">Dear {message.PassengerName}, your reservation has been successfully processed.</p>
      <div style=""margin:20px 0;padding:20px;background-color:#f0f5ff;border-radius:6px;border:1px solid #d0e4ff;"">
        <h3 style=""color:#003580;margin:0 0 15px;"">Booking Summary</h3>
        <table style=""width:100%;border-collapse:collapse;"">
          <tr><td style=""color:#777;padding:6px 0;"">PNR Number</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.PnrNumber}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">Flight</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.FlightNumber}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">Route</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureCity} &#8594; {message.ArrivalCity}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">Departure</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.DepartureDateTime:dd MMM yyyy HH:mm}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">Seat</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.SeatNumber} ({message.SeatClass})</td></tr>
          <tr style=""border-top:1px solid #d0e4ff;""><td style=""color:#003580;padding:10px 0 0;font-weight:bold;"">Total Price</td><td style=""color:#003580;font-weight:bold;text-align:right;padding-top:10px;font-size:18px;"">{message.TotalPrice:C2}</td></tr>
        </table>
      </div>
      <p style=""color:#555;font-size:13px;"">Please keep your PNR number safe. You will need it for check-in and any changes to your booking.</p>
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
                Subject = $"{_localizer[Messages.EmailSubject_BookingConfirmation]} - {message.PnrNumber}",
                Body = body,
                IsHtml = true
            });
        }

        private async Task SendConfirmationSmsAsync(BookingConfirmedEvent message)
        {
            var smsBody = $"Booking confirmed! PNR: {message.PnrNumber} | Flight: {message.FlightNumber} | {message.DepartureCity}-{message.ArrivalCity} | {message.DepartureDateTime:dd MMM yyyy HH:mm} | Seat: {message.SeatNumber} | Total: {message.TotalPrice:C2}";
            await _smsSenderService.SendSmsAsync(message.PhoneNumber, smsBody);
        }

        private async Task SendConfirmationWhatsAppAsync(BookingConfirmedEvent message)
        {
            var whatsAppBody = $"✈ *Booking Confirmed!*\n\n" +
                               $"*PNR:* {message.PnrNumber}\n" +
                               $"*Flight:* {message.FlightNumber}\n" +
                               $"*Route:* {message.DepartureCity} → {message.ArrivalCity}\n" +
                               $"*Departure:* {message.DepartureDateTime:dd MMM yyyy HH:mm}\n" +
                               $"*Seat:* {message.SeatNumber} ({message.SeatClass})\n" +
                               $"*Total:* {message.TotalPrice:C2}\n\n" +
                               $"Please keep your PNR number safe for check-in.";
            await _smsSenderService.SendWhatsAppAsync(message.PhoneNumber, whatsAppBody);
        }
    }
}

