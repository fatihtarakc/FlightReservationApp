namespace App.Web.Services.Concrete
{
    public class AdminWebService : IAdminWebService
    {
        private readonly IApiHttpClient _api;

        public AdminWebService(IApiHttpClient api)
        {
            _api = api;
        }

        public Task<ApiResponse<DashboardViewModel>?> GetDashboardAsync(string token) =>
            _api.GetAsync<DashboardViewModel>("admin/dashboard", token);

        public Task<ApiResponse<List<FlightPassengerStatViewModel>>?> GetFlightPassengerStatsAsync(string token) =>
            _api.GetAsync<List<FlightPassengerStatViewModel>>("admin/flight-passenger-stats", token);

        public Task<ApiResponse<List<AdminBookingListItemViewModel>>?> GetAllBookingsAsync(string token) =>
            _api.GetAsync<List<AdminBookingListItemViewModel>>("booking", token);
    }
}
