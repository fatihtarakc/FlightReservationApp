namespace App.Web.Services.Interfaces
{
    public interface IFlightService
    {
        Task<IDataResult<List<FlightVM>>> GetAllAsync();
        Task<IDataResult<FlightVM>> GetByIdAsync(Guid id);
        Task<IDataResult<List<FlightVM>>> SearchAsync(FlightSearchVM model);
        Task<IDataResult<FlightDetailPageVM>> GetDetailWithSeatsAsync(Guid id);
        Task<IDataResult<FlightVM>> AddAsync(FlightAddVM model, string token);
        Task<IDataResult<FlightVM>> UpdateAsync(Guid id, FlightAddVM model, string token);
        Task<IResult> CancelAsync(Guid id, string? reason, string token);
        Task<IResult> DeleteAsync(Guid id, string token);
    }
}
