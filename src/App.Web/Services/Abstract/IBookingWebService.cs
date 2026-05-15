namespace App.Web.Services.Abstract
{
    public interface IBookingWebService
    {
        Task<ApiResponse<List<BookingListItemViewModel>>?> GetMyBookingsAsync(string token);
        Task<ApiResponse<BookingDetailsViewModel>?> GetDetailsAsync(Guid id, string token);
        Task<ApiResponse<BookingDetailsViewModel>?> BookSeatAsync(Guid flightId, Guid seatId, string token);
        Task<ApiResponse<object>?> CancelAsync(Guid id, string token);
    }
}
