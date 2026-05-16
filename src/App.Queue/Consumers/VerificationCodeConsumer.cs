namespace App.Queue.Consumers
{
    public class VerificationCodeConsumer : IConsumer<VerificationCodeEvent>
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<VerificationCodeConsumer> _logger;

        public VerificationCodeConsumer(
            IEmailSenderService emailSenderService,
            ISmsSenderService smsSenderService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<VerificationCodeConsumer> logger)
        {
            _emailSenderService = emailSenderService;
            _smsSenderService = smsSenderService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<VerificationCodeEvent> context)
        {
            var message = context.Message;
            CultureInfo.CurrentUICulture = new CultureInfo(message.Language);

            _logger.LogInformation("Processing VerificationCodeEvent for Email: {Email} Purpose: {Purpose} Channel: {Channel}",
                message.Email, message.Purpose, message.Channel);

            switch (message.Channel)
            {
                case VerificationCodeChannel.Email:
                    await SendVerificationEmailAsync(message);
                    break;
                case VerificationCodeChannel.Sms:
                    await SendVerificationSmsAsync(message);
                    break;
                case VerificationCodeChannel.WhatsApp:
                    await SendVerificationWhatsAppAsync(message);
                    break;
            }

            _logger.LogInformation("{Message} Email: {Email} Purpose: {Purpose}",
                _localizer[Messages.MassTransit_Consume_Was_Successful].Value, message.Email, message.Purpose);
        }

        private async Task SendVerificationEmailAsync(VerificationCodeEvent message)
        {
            var purposeLabel = GetPurposeLabel(message.Purpose);
            var dear = _localizer[Messages.Email_Dear].Value;
            var footer = _localizer[Messages.Email_Footer].Value;

            var subjectKey = message.Purpose == VerificationCodePurpose.PasswordReset
                ? Messages.EmailSubject_PasswordReset
                : Messages.EmailSubject_VerificationCode;

            var body = $@"<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""></head>
<body style=""font-family:Arial,sans-serif;background-color:#f4f4f4;margin:0;padding:20px;"">
  <div style=""max-width:600px;margin:0 auto;background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,.1);"">
    <div style=""background-color:#003580;padding:30px;text-align:center;"">
      <h1 style=""color:#ffffff;margin:0;font-size:24px;"">&#9992; Flight Reservation</h1>
      <p style=""color:#a8c4e0;margin:8px 0 0;"">{_localizer[Messages.Email_VerificationCode_HeaderSubtitle]}</p>
    </div>
    <div style=""padding:30px;"">
      <h2 style=""color:#003580;"">{purposeLabel}</h2>
      <p style=""color:#555;line-height:1.7;"">{dear} {message.Name}, {_localizer[Messages.Email_VerificationCode_Intro]}</p>
      <div style=""text-align:center;margin:30px 0;"">
        <span style=""display:inline-block;background-color:#e8f0fe;color:#003580;font-size:36px;font-weight:bold;letter-spacing:12px;padding:20px 30px;border-radius:8px;border:2px solid #003580;"">{message.Code}</span>
      </div>
      <div style=""padding:14px;background-color:#fff3cd;border-radius:4px;border-left:4px solid #ffc107;"">
        <p style=""margin:0;color:#856404;font-size:13px;"">&#9888; {_localizer[Messages.Email_VerificationCode_Warning]}</p>
      </div>
      <p style=""color:#888;font-size:12px;margin-top:20px;"">{_localizer[Messages.Email_VerificationCode_NotRequested]}</p>
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
                Subject = $"{_localizer[subjectKey]} - {purposeLabel}",
                Body = body,
                IsHtml = true
            });
        }

        private async Task SendVerificationSmsAsync(VerificationCodeEvent message)
        {
            var purposeLabel = GetPurposeLabel(message.Purpose);
            var smsBody = string.Format(_localizer[Messages.Sms_VerificationCode].Value, purposeLabel, message.Code);
            await _smsSenderService.SendSmsAsync(message.PhoneNumber, smsBody);
        }

        private async Task SendVerificationWhatsAppAsync(VerificationCodeEvent message)
        {
            var purposeLabel = GetPurposeLabel(message.Purpose);
            var whatsAppBody = string.Format(_localizer[Messages.WhatsApp_VerificationCode].Value, purposeLabel, message.Code)
                .Replace("\\n", "\n");
            await _smsSenderService.SendWhatsAppAsync(message.PhoneNumber, whatsAppBody);
        }

        private string GetPurposeLabel(VerificationCodePurpose purpose) => purpose switch
        {
            VerificationCodePurpose.EmailConfirmation       => CultureInfo.CurrentUICulture.Name.StartsWith("en")
                                                                ? "Email Verification" : "E-posta Doğrulama",
            VerificationCodePurpose.PasswordReset           => CultureInfo.CurrentUICulture.Name.StartsWith("en")
                                                                ? "Password Reset" : "Şifre Sıfırlama",
            VerificationCodePurpose.TwoFactorAuthentication => CultureInfo.CurrentUICulture.Name.StartsWith("en")
                                                                ? "Two-Factor Authentication" : "İki Faktörlü Kimlik Doğrulama",
            VerificationCodePurpose.AccountActivation       => CultureInfo.CurrentUICulture.Name.StartsWith("en")
                                                                ? "Account Activation" : "Hesap Aktivasyonu",
            VerificationCodePurpose.PhoneVerification       => CultureInfo.CurrentUICulture.Name.StartsWith("en")
                                                                ? "Phone Verification" : "Telefon Doğrulama",
            _                                               => CultureInfo.CurrentUICulture.Name.StartsWith("en")
                                                                ? "Verification" : "Doğrulama"
        };
    }
}
