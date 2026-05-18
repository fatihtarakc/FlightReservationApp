namespace App.Web.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<List<ScheduleSelectItemVM>> GetAllAsync();
        Task<IDataResult<List<ScheduleVM>>> GetAllFullAsync();
        Task<IDataResult<ScheduleVM>> GetByIdAsync(Guid id);
        Task<IDataResult<ScheduleVM>> AddAsync(ScheduleAddVM model, string token);
        Task<IResult> UpdateAsync(Guid id, ScheduleUpdateVM model, string token);
        Task<IResult> DeleteAsync(Guid id, string token);
        Task<bool> HasFlightsAsync(Guid id);
    }
}
