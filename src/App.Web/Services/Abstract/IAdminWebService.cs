namespace App.Web.Services.Abstract
{
    public interface IAdminWebService
    {
        Task<ApiResponse<DashboardViewModel>?> GetDashboardAsync(string token);
        Task<ApiResponse<List<FlightPassengerStatViewModel>>?> GetFlightPassengerStatsAsync(string token);
        Task<ApiResponse<List<AdminBookingListItemViewModel>>?> GetAllBookingsAsync(string token);
    }
}
