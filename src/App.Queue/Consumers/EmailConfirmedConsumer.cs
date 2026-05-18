namespace App.Queue.Consumers
{
    public class EmailConfirmedConsumer : IConsumer<EmailConfirmedEvent>
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<EmailConfirmedConsumer> _logger;

        public EmailConfirmedConsumer(
            IEmailSenderService emailSenderService,
            ISmsSenderService smsSenderService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<EmailConfirmedConsumer> logger)
        {
            _emailSenderService = emailSenderService;
            _smsSenderService = smsSenderService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EmailConfirmedEvent> context)
        {
            var message = context.Message;
            CultureInfo.CurrentUICulture = new CultureInfo(message.Language);

            _logger.LogInformation("Processing EmailConfirmedEvent for Email: {Email}", message.Email);

            var tasks = new List<Task>();

            if (message.PreferredChannel.HasFlag(NotificationChannel.Email))
                tasks.Add(SendEmailConfirmedEmailAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.Sms))
                tasks.Add(SendEmailConfirmedSmsAsync(message));

            if (message.PreferredChannel.HasFlag(NotificationChannel.WhatsApp))
                tasks.Add(SendEmailConfirmedWhatsAppAsync(message));

            await Task.WhenAll(tasks);
            _logger.LogInformation("{Message} Email: {Email}", _localizer[Messages.MassTransit_Consume_Was_Successful].Value, message.Email);
        }

        private async Task SendEmailConfirmedEmailAsync(EmailConfirmedEvent message)
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
    }}
  </style>
</head>
<body class=""email-body"" style=""font-family:Arial,sans-serif;background-color:#f4f4f4;margin:0;padding:20px;"">
  <div class=""email-wrap"" style=""max-width:600px;margin:0 auto;background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,.1);"">
    <div style=""background-color:#27ae60;padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; Flight Reservation</h1>
      <p style=""color:rgba(255,255,255,.85);margin:8px 0 0;"">{_localizer[Messages.Email_EmailConfirmed_HeaderSubtitle]}</p>
    </div>
    <div class=""email-content"" style=""padding:30px;"">
      <div style=""text-align:center;margin-bottom:24px;"">
        <span style=""display:inline-block;background-color:#27ae60;color:#fff;width:64px;height:64px;border-radius:50%;font-size:32px;line-height:64px;"">&#10003;</span>
      </div>
      <h2 style=""color:#27ae60;text-align:center;margin:0 0 16px;"">{_localizer[Messages.Email_EmailConfirmed_Heading]}</h2>
      <p style=""color:#555;line-height:1.7;"">{dear} <strong>{message.Name}</strong>, {_localizer[Messages.Email_EmailConfirmed_Intro]}</p>
      <div style=""padding:16px;background-color:#f0fff4;border-radius:4px;border-left:4px solid #27ae60;margin-top:20px;"">
        <p style=""margin:0;color:#555;font-size:13px;"">&#9888; {_localizer[Messages.Email_EmailConfirmed_Note]}</p>
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
                Subject = _localizer[Messages.EmailSubject_EmailConfirmed].Value,
                Body = body,
                IsHtml = true
            });
        }

        private async Task SendEmailConfirmedSmsAsync(EmailConfirmedEvent message)
        {
            var smsBody = _localizer[Messages.Sms_EmailConfirmed].Value;
            await _smsSenderService.SendSmsAsync(message.PhoneNumber, smsBody);
        }

        private async Task SendEmailConfirmedWhatsAppAsync(EmailConfirmedEvent message)
        {
            var whatsAppBody = _localizer[Messages.WhatsApp_EmailConfirmed].Value.Replace("\\n", "\n");
            await _smsSenderService.SendWhatsAppAsync(message.PhoneNumber, whatsAppBody);
        }
    }
}
