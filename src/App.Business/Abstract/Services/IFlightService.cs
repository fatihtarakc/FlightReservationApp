namespace App.Business.Abstract.Services
{
    public interface IFlightService
    {
        Task<IDataResult<FlightDto>> GetByIdAsync(Guid id);
        Task<IDataResult<IEnumerable<FlightListDto>>> GetAllAsync();
        Task<IDataResult<IEnumerable<FlightListDto>>> SearchAsync(FlightSearchDto dto);
        Task<IDataResult<FlightDto>> AddAsync(FlightAddDto dto);
        Task<IResult> UpdateAsync(Guid id, FlightUpdateDto dto);
        Task<IResult> CancelAsync(Guid id, string? reason);
        Task<IResult> DeleteAsync(Guid id);
    }
}
