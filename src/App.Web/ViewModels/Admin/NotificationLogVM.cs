namespace App.Web.ViewModels.Admin
{
    public class NotificationLogVM
    {
        public Guid Id { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime SentAt { get; set; }
        public string? RelatedEntityType { get; set; }
        public string? RelatedEntityId { get; set; }
    }

    public class AppLogVM
    {
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public DateTime Timestamp { get; set; }
        public string? SourceContext { get; set; }
        public string? RequestPath { get; set; }
    }
}
