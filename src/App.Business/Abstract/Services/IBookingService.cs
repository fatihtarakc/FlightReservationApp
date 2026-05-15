namespace App.Business.Abstract.Services
{
    public interface IBookingService
    {
        Task<IDataResult<BookingDto>> GetByIdAsync(Guid id);
        Task<IDataResult<BookingDto>> GetByPnrAsync(string pnr);
        Task<IDataResult<IEnumerable<BookingListDto>>> GetByUserIdAsync(Guid userId);
        Task<IDataResult<IEnumerable<BookingDto>>> GetAllAsync();
        Task<IDataResult<IEnumerable<BookingListDto>>> GetByFlightIdAsync(Guid flightId);
        Task<IDataResult<BookingDto>> AddAsync(Guid userId, BookingAddDto dto);
        Task<IResult> CancelAsync(Guid id, string? reason);
    }
}
