namespace App.BackgroundJobs.Schedules
{
    public static class RecurringJobSchedules
    {
        public const string FlightReminder7Days = "flight-reminder-7days";
        public const string FlightReminder24Hours = "flight-reminder-24hours";

        // Every day at 08:00 UTC
        public const string DailyAt8AM = "0 8 * * *";

        // Every hour at minute 0
        public const string Hourly = "0 * * * *";
    }
}
