namespace App.Web.ViewModels.Admin
{
    public class HangfireStatsVM
    {
        public long SucceededCount { get; set; }
        public long EnqueuedCount { get; set; }
        public long ProcessingCount { get; set; }
        public long FailedCount { get; set; }
    }
}
