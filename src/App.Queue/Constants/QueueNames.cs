namespace App.Queue.Constants
{
    public static class QueueNames
    {
        public const string SendEmail = "flight-reservation-send-email";
        public const string SendSms = "flight-reservation-send-sms";
        public const string SendWhatsApp = "flight-reservation-send-whatsapp";
        public const string UserRegistered = "flight-reservation-user-registered";
        public const string BookingConfirmed = "flight-reservation-booking-confirmed";
        public const string BookingCancelled = "flight-reservation-booking-cancelled";
        public const string FlightCancelled = "flight-reservation-flight-cancelled";
        public const string FlightReminder = "flight-reservation-flight-reminder";
        public const string VerificationCode = "flight-reservation-verification-code";
    }
}
