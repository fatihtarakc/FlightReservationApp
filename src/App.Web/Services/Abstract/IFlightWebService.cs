namespace App.Web.Services.Abstract
{
    public interface IFlightWebService
    {
        Task<ApiResponse<List<FlightListItemViewModel>>?> SearchAsync(
            string departureIata, string arrivalIata,
            DateTime departureDate, int passengers, int seatClass, string? token = null);

        Task<ApiResponse<FlightDetailsViewModel>?> GetDetailsAsync(Guid id, string? token = null);

        Task<ApiResponse<List<SeatViewModel>>?> GetAllSeatsWithAvailabilityAsync(Guid flightId, string? token = null);
    }
}
