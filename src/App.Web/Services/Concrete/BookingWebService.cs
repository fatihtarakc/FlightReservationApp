namespace App.Web.Services.Concrete
{
    public class BookingWebService : IBookingWebService
    {
        private readonly IApiHttpClient _api;

        public BookingWebService(IApiHttpClient api)
        {
            _api = api;
        }

        public Task<ApiResponse<List<BookingListItemViewModel>>?> GetMyBookingsAsync(string token) =>
            _api.GetAsync<List<BookingListItemViewModel>>("booking/my-bookings", token);

        public Task<ApiResponse<BookingDetailsViewModel>?> GetDetailsAsync(Guid id, string token) =>
            _api.GetAsync<BookingDetailsViewModel>($"booking/{id}", token);

        public Task<ApiResponse<BookingDetailsViewModel>?> BookSeatAsync(Guid flightId, Guid seatId, string token) =>
            _api.PostAsync<BookingDetailsViewModel>("booking", new { FlightId = flightId, SeatId = seatId }, token);

        public Task<ApiResponse<object>?> CancelAsync(Guid id, string token) =>
            _api.PostAsync<object>($"booking/{id}/cancel", new { }, token);
    }
}
