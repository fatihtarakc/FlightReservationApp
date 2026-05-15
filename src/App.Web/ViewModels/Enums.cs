namespace App.Web.Enums
{
    public enum SeatClass
    {
        Economy = 1,
        PremiumEconomy = 2,
        Business = 3,
        First = 4
    }

    public enum SeatColumn
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5,
        F = 6,
        G = 7,
        H = 8,
        J = 9,
        K = 10
    }

    public enum BodyType
    {
        NarrowBody = 1,
        WideBody = 2,
        RegionalJet = 3,
        Turboprop = 4
    }

    public enum UserStatus
    {
        Pending = 1,
        Active = 2,
        Suspended = 3,
        Deactivated = 4
    }

    public enum BookingStatus
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3,
        CheckedIn = 4,
        Boarded = 5,
        Completed = 6,
        NoShow = 7
    }

    public enum FlightStatus
    {
        Scheduled = 1,
        Boarding = 2,
        Departed = 3,
        InAir = 4,
        Landed = 5,
        Arrived = 6,
        Delayed = 7,
        Cancelled = 8,
        Diverted = 9
    }

    [Flags]
    public enum NotificationChannel
    {
        None = 0,
        Email = 1,
        Sms = 2,
        WhatsApp = 4,
        All = Email | Sms | WhatsApp
    }

    public enum Currency
    {
        TRY = 1,
        USD = 2,
        EUR = 3,
        GBP = 4,
        AED = 5
    }
}
