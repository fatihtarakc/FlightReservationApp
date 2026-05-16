namespace App.Web.Services.Interfaces
{
    public interface IRouteService
    {
        Task<IDataResult<List<RouteVM>>> GetAllAsync();
        Task<IDataResult<RouteVM>> GetByIdAsync(Guid id);
        Task<IDataResult<RouteVM>> AddAsync(RouteAddVM model, string token);
        Task<IResult> UpdateAsync(Guid id, RouteUpdateVM model, string token);
        Task<IResult> DeleteAsync(Guid id, string token);
    }
}
