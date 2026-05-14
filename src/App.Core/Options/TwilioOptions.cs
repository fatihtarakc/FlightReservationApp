namespace App.Core.Options
{
    public class TwilioOptions
    {
        public const string TwilioConfiguration = "TwilioConfiguration";

        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string FromPhoneNumber { get; set; } = string.Empty;
        public string WhatsAppFromNumber { get; set; } = string.Empty;
    }
}
