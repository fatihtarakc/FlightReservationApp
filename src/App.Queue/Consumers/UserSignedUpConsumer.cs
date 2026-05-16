namespace App.Queue.Consumers
{
    public class UserSignedUpConsumer : IConsumer<UserSignedUpEvent>
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<UserSignedUpConsumer> _logger;

        public UserSignedUpConsumer(
            IEmailSenderService emailSenderService,
            ISmsSenderService smsSenderService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<UserSignedUpConsumer> logger)
        {
            _emailSenderService = emailSenderService;
            _smsSenderService = smsSenderService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserSignedUpEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Processing UserSignedUpEvent for AppUserId: {AppUserId}", message.AppUserId);

            var tasks = new List<Task>();

            if (message.PreferredChannel.HasFlag(NotificationChannel.Email))
                tasks.Add(SendWelcomeEmailAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.Sms))
                tasks.Add(SendWelcomeSmsAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.WhatsApp))
                tasks.Add(SendWelcomeWhatsAppAsync(message));

            await Task.WhenAll(tasks);
            _logger.LogInformation("{Message} AppUserId: {AppUserId}", _localizer[Messages.MassTransit_Consume_Was_Successful].Value, message.AppUserId);
        }

        private async Task SendWelcomeEmailAsync(UserSignedUpEvent message)
        {
            var body = $@"<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""></head>
<body style=""font-family:Arial,sans-serif;background-color:#f4f4f4;margin:0;padding:20px;"">
  <div style=""max-width:600px;margin:0 auto;background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,.1);"">
    <div style=""background-color:#003580;padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; FlightReservation</h1>
    </div>
    <div style=""padding:30px;"">
      <h2 style=""color:#003580;"">Welcome, {message.Name} {message.Surname}!</h2>
      <p style=""color:#555;line-height:1.7;"">Your account has been successfully created. You can now search for flights, make reservations, and manage your bookings seamlessly.</p>
      <p style=""color:#555;line-height:1.7;"">We look forward to serving you on your next journey.</p>
      <div style=""margin:30px 0;padding:20px;background-color:#f0f5ff;border-left:4px solid #003580;border-radius:4px;"">
        <p style=""margin:0;color:#003580;font-weight:bold;"">Account Details</p>
        <p style=""margin:8px 0 0;color:#555;"">Name: {message.Name} {message.Surname}</p>
        <p style=""margin:4px 0 0;color:#555;"">Email: {message.Email}</p>
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
                To = message.Email,
                Subject = _localizer[Messages.EmailSubject_WelcomeNewUser],
                Body = body,
                IsHtml = true
            });
        }

        private async Task SendWelcomeSmsAsync(UserSignedUpEvent message)
        {
            var smsBody = $"Welcome to FlightReservation, {message.Name} {message.Surname}! Your account has been created. Start exploring flights now.";
            await _smsSenderService.SendSmsAsync(message.PhoneNumber, smsBody);
        }

        private async Task SendWelcomeWhatsAppAsync(UserSignedUpEvent message)
        {
            var whatsAppBody = $"✈ Welcome to FlightReservation, {message.Name} {message.Surname}!\n\nYour account has been successfully created.\n\nStart exploring flights and making reservations at any time.";
            await _smsSenderService.SendWhatsAppAsync(message.PhoneNumber, whatsAppBody);
        }
    }
}

