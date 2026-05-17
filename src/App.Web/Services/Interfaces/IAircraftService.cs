namespace App.Web.Services.Interfaces
{
    public interface IAircraftService
    {
        Task<IDataResult<List<AircraftVM>>>       GetAllAsync();
        Task<IDataResult<AircraftVM>>             GetByIdAsync(Guid id);
        Task<IDataResult<AircraftVM>>             AddAsync(AircraftAddVM model, string token);
        Task<IDataResult<AircraftVM>>             UpdateAsync(Guid id, AircraftUpdateVM model, string token);
        Task<IResult>                             DeleteAsync(Guid id, string token);
        Task<List<AircraftSelectItemVM>>          GetAirlinesAsync();
        Task<List<AircraftSelectItemVM>>          GetModelsAsync();
    }
}
