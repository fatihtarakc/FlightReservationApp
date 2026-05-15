namespace App.Web.Enums
{
    [Flags]
    public enum NotificationChannel
    {
        None = 0,
        Email = 1,
        Sms = 2,
        WhatsApp = 4,
        All = Email | Sms | WhatsApp
    }
}
