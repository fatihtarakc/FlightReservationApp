namespace App.Web.Services.Interfaces
{
    public interface ISeatService
    {
        Task<IDataResult<List<SeatVM>>> GetByFlightIdAsync(Guid flightId);
        Task<IDataResult<SeatVM>> GetByIdAsync(Guid id);
    }
}
