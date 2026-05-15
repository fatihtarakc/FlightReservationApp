namespace App.Web.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IDataResult<List<BookingVM>>> GetAllAsync(string token);
        Task<IDataResult<List<BookingVM>>> GetMyBookingsAsync(string token);
        Task<IDataResult<BookingVM>> GetByIdAsync(Guid id, string token);
        Task<IDataResult<BookingVM>> CreateAsync(BookingAddVM model, string token);
        Task<IResult> CancelAsync(Guid id, string token);
        Task<IResult> CheckInAsync(Guid id, string token);
    }
}
