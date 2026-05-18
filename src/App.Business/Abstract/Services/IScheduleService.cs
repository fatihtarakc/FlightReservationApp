namespace App.Business.Abstract.Services
{
    public interface IScheduleService
    {
        Task<IDataResult<ScheduleDto>> GetByIdAsync(Guid id);
        Task<IDataResult<IEnumerable<ScheduleDto>>> GetAllAsync();
        Task<IDataResult<IEnumerable<ScheduleDto>>> GetByRouteIdAsync(Guid routeId);
        Task<IDataResult<ScheduleDto>> AddAsync(ScheduleAddDto dto);
        Task<IResult> UpdateAsync(Guid id, ScheduleUpdateDto dto);
        Task<IResult> DeleteAsync(Guid id);
        Task<bool> HasFlightsAsync(Guid id);
    }
}
