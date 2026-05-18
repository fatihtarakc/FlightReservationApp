namespace App.Web.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<List<ScheduleSelectItemVM>> GetAllAsync();
    }
}
