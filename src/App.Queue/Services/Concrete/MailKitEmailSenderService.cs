using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace App.Queue.Services.Concrete
{
    public class MailKitEmailSenderService : IEmailSenderService
    {
        private readonly EmailOptions _emailOptions;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<MailKitEmailSenderService> _logger;

        public MailKitEmailSenderService(
            IOptions<EmailOptions> emailOptions,
            IStringLocalizer<MessageResources> localizer,
            ILogger<MailKitEmailSenderService> logger)
        {
            _emailOptions = emailOptions.Value;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IResult> SendAsync(EmailDto emailDto)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailOptions.From, _emailOptions.EmailFrom));
                message.To.Add(MailboxAddress.Parse(emailDto.To));
                message.Subject = emailDto.Subject;
                message.Body = new TextPart(emailDto.IsHtml ? TextFormat.Html : TextFormat.Plain)
                {
                    Text = emailDto.Body
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _emailOptions.SmtpServer,
                    _emailOptions.Port,
                    SecureSocketOptions.Auto);
                await client.AuthenticateAsync(_emailOptions.Username, _emailOptions.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("{Message} To: {To}", _localizer[Messages.Email_SendingProcess_Was_Successful].Value, emailDto.To);
                return new SuccessResult(_localizer[Messages.Email_SendingProcess_Was_Successful]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message} To: {To}", _localizer[Messages.Email_SendingProcess_Was_Failed].Value, emailDto.To);
                return new ErrorResult(_localizer[Messages.Email_SendingProcess_Was_Failed]);
            }
        }
    }
}
