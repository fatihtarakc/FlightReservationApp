namespace App.Web.Services.Concrete
{
    public class FlightWebService : IFlightWebService
    {
        private readonly IApiHttpClient _api;

        public FlightWebService(IApiHttpClient api)
        {
            _api = api;
        }

        public Task<ApiResponse<List<FlightListItemViewModel>>?> SearchAsync(
            string departureIata, string arrivalIata,
            DateTime departureDate, int passengers, int seatClass, string? token = null)
        {
            var qs = $"flight/search?departureIata={Uri.EscapeDataString(departureIata)}" +
                     $"&arrivalIata={Uri.EscapeDataString(arrivalIata)}" +
                     $"&departureDate={departureDate:yyyy-MM-dd}" +
                     $"&passengers={passengers}" +
                     $"&seatClass={seatClass}";
            return _api.GetAsync<List<FlightListItemViewModel>>(qs, token);
        }

        public Task<ApiResponse<FlightDetailsViewModel>?> GetDetailsAsync(Guid id, string? token = null) =>
            _api.GetAsync<FlightDetailsViewModel>($"flight/{id}", token);

        public Task<ApiResponse<List<SeatViewModel>>?> GetAllSeatsWithAvailabilityAsync(Guid flightId, string? token = null) =>
            _api.GetAsync<List<SeatViewModel>>($"seat/flight/{flightId}", token);
    }
}
