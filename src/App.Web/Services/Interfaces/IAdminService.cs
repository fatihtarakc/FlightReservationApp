namespace App.Web.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IDataResult<AdminDashboardVM>> GetDashboardAsync(string token);
        Task<IDataResult<List<NotificationLogVM>>> GetNotificationLogsAsync(string token, string? search, string? channel, string? date);
        Task<IDataResult<List<AppLogVM>>> GetAppLogsAsync(string token, string? search, string? level, string? date);
        Task<IDataResult<HangfireStatsVM>> GetHangfireStatsAsync(string token);
    }
}
