namespace App.Queue.Services.Concrete
{
    public class TwilioSmsSenderService : ISmsSenderService
    {
        private readonly TwilioOptions _twilioOptions;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<TwilioSmsSenderService> _logger;

        public TwilioSmsSenderService(
            IOptions<TwilioOptions> twilioOptions,
            IStringLocalizer<MessageResources> localizer,
            ILogger<TwilioSmsSenderService> logger)
        {
            _twilioOptions = twilioOptions.Value;
            _localizer = localizer;
            _logger = logger;
            TwilioClient.Init(_twilioOptions.AccountSid, _twilioOptions.AuthToken);
        }

        public async Task<IResult> SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {
                var smsMessage = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(_twilioOptions.FromPhoneNumber),
                    to: new PhoneNumber(toPhoneNumber));

                _logger.LogInformation("{Message} To: {To} SID: {Sid}",
                    _localizer[Messages.Sms_SendingProcess_Was_Successful].Value, toPhoneNumber, smsMessage.Sid);
                return new SuccessResult(_localizer[Messages.Sms_SendingProcess_Was_Successful]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message} To: {To}", _localizer[Messages.Sms_SendingProcess_Was_Failed].Value, toPhoneNumber);
                return new ErrorResult(_localizer[Messages.Sms_SendingProcess_Was_Failed]);
            }
        }

        public async Task<IResult> SendWhatsAppAsync(string toPhoneNumber, string message)
        {
            try
            {
                var whatsAppMessage = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber($"whatsapp:{_twilioOptions.WhatsAppFromNumber}"),
                    to: new PhoneNumber($"whatsapp:{toPhoneNumber}"));

                _logger.LogInformation("{Message} To: {To} SID: {Sid}",
                    _localizer[Messages.WhatsApp_SendingProcess_Was_Successful].Value, toPhoneNumber, whatsAppMessage.Sid);
                return new SuccessResult(_localizer[Messages.WhatsApp_SendingProcess_Was_Successful]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message} To: {To}", _localizer[Messages.WhatsApp_SendingProcess_Was_Failed].Value, toPhoneNumber);
                return new ErrorResult(_localizer[Messages.WhatsApp_SendingProcess_Was_Failed]);
            }
        }
    }
}
