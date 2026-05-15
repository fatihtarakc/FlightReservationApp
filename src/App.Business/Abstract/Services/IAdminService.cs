namespace App.Business.Abstract.Services
{
    public interface IAdminService
    {
        Task<IDataResult<AdminDashboardDto>> GetDashboardAsync();
        Task<IDataResult<IEnumerable<FlightPassengerStatDto>>> GetFlightPassengerStatsAsync();
    }
}
