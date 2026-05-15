namespace App.Business.Abstract.Services
{
    public interface ISeatService
    {
        Task<IDataResult<SeatDto>> GetByIdAsync(Guid id);
        Task<IDataResult<IEnumerable<SeatDto>>> GetByAircraftIdAsync(Guid aircraftId);
        Task<IDataResult<IEnumerable<SeatDto>>> GetAvailableByFlightIdAsync(Guid flightId);
        Task<IDataResult<IEnumerable<SeatDto>>> GetAllWithAvailabilityByFlightIdAsync(Guid flightId);
        Task<IResult> AddAsync(SeatAddDto dto);
        Task<IResult> AddRangeAsync(IEnumerable<SeatAddDto> dtos);
        Task<IResult> DeleteAsync(Guid id);
    }
}
