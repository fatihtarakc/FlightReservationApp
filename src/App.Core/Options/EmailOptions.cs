namespace App.Core.Options
{
    public class EmailOptions
    {
        public const string EmailConfiguration = "EmailConfiguration";

        public string From { get; set; } = string.Empty;
        public string EmailFrom { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseSsl { get; set; }
    }
}
