namespace App.Queue.Consumers
{
    public class PasswordChangedConsumer : IConsumer<PasswordChangedEvent>
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<PasswordChangedConsumer> _logger;

        public PasswordChangedConsumer(
            IEmailSenderService emailSenderService,
            ISmsSenderService smsSenderService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<PasswordChangedConsumer> logger)
        {
            _emailSenderService = emailSenderService;
            _smsSenderService = smsSenderService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PasswordChangedEvent> context)
        {
            var message = context.Message;
            CultureInfo.CurrentUICulture = new CultureInfo(message.Language);

            _logger.LogInformation("Processing PasswordChangedEvent for Email: {Email}", message.Email);

            var tasks = new List<Task>();

            if (message.PreferredChannel.HasFlag(NotificationChannel.Email))
                tasks.Add(SendPasswordChangedEmailAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.Sms))
                tasks.Add(SendPasswordChangedSmsAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.WhatsApp))
                tasks.Add(SendPasswordChangedWhatsAppAsync(message));

            await Task.WhenAll(tasks);
            _logger.LogInformation("{Message} Email: {Email}", _localizer[Messages.MassTransit_Consume_Was_Successful].Value, message.Email);
        }

        private async Task SendPasswordChangedEmailAsync(PasswordChangedEvent message)
        {
            var dear = _localizer[Messages.Email_Dear].Value;
            var footer = _localizer[Messages.Email_Footer].Value;
            var changedAtFormatted = message.ChangedAt.ToLocalTime().ToString("dd MMM yyyy HH:mm");

            var body = $@"<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""></head>
<body style=""font-family:Arial,sans-serif;background-color:#f4f4f4;margin:0;padding:20px;"">
  <div style=""max-width:600px;margin:0 auto;background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,.1);"">
    <div style=""background-color:#003580;padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; Flight Reservation</h1>
      <p style=""color:#a8c4e0;margin:8px 0 0;"">{_localizer[Messages.Email_PasswordChanged_HeaderSubtitle]}</p>
    </div>
    <div style=""padding:30px;"">
      <h2 style=""color:#003580;"">{_localizer[Messages.Email_PasswordChanged_Heading]}</h2>
      <p style=""color:#555;line-height:1.7;"">{dear} {message.Name}, {_localizer[Messages.Email_PasswordChanged_Intro]}</p>
      <div style=""margin:20px 0;padding:20px;background-color:#f0f5ff;border-radius:6px;border:1px solid #d0e4ff;"">
        <table style=""width:100%;border-collapse:collapse;"">
          <tr><td style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_Email]}</td><td style=""color:#222;font-weight:bold;text-align:right;"">{message.Email}</td></tr>
          <tr><td style=""color:#777;padding:6px 0;"">{_localizer[Messages.Email_Label_OperationTime]}</td><td style=""color:#222;font-weight:bold;text-align:right;"">{changedAtFormatted}</td></tr>
        </table>
      </div>
      <div style=""padding:16px;background-color:#fff3cd;border-radius:4px;border-left:4px solid #ffc107;"">
        <p style=""margin:0;color:#856404;font-size:13px;"">&#9888; {_localizer[Messages.Email_PasswordChanged_Warning]}</p>
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
                To = message.Email,
                Subject = _localizer[Messages.EmailSubject_PasswordChanged].Value,
                Body = body,
                IsHtml = true
            });
        }

        private async Task SendPasswordChangedSmsAsync(PasswordChangedEvent message)
        {
            var changedAtFormatted = message.ChangedAt.ToLocalTime().ToString("dd MMM yyyy HH:mm");
            var smsBody = string.Format(_localizer[Messages.Sms_PasswordChanged].Value, changedAtFormatted);
            await _smsSenderService.SendSmsAsync(message.PhoneNumber, smsBody);
        }

        private async Task SendPasswordChangedWhatsAppAsync(PasswordChangedEvent message)
        {
            var changedAtFormatted = message.ChangedAt.ToLocalTime().ToString("dd MMM yyyy HH:mm");
            var whatsAppBody = string.Format(_localizer[Messages.WhatsApp_PasswordChanged].Value, changedAtFormatted)
                .Replace("\\n", "\n");
            await _smsSenderService.SendWhatsAppAsync(message.PhoneNumber, whatsAppBody);
        }
    }
}
