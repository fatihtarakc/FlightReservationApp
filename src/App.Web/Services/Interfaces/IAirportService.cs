namespace App.Web.Services.Interfaces
{
    public interface IAirportService
    {
        Task<IDataResult<List<AirportVM>>> GetAllAsync();
        Task<IDataResult<AirportVM>> GetByIdAsync(Guid id);
        Task<IDataResult<AirportVM>> AddAsync(AirportAddVM model, string token);
        Task<IDataResult<AirportVM>> UpdateAsync(Guid id, AirportAddVM model, string token);
        Task<IResult> DeleteAsync(Guid id, string token);
    }
}
