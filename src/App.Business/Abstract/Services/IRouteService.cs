namespace App.Business.Abstract.Services
{
    public interface IRouteService
    {
        Task<IDataResult<RouteDto>> GetByIdAsync(Guid id);
        Task<IDataResult<IEnumerable<RouteDto>>> GetAllAsync();
        Task<IDataResult<RouteDto>> AddAsync(RouteAddDto dto);
        Task<IResult> UpdateAsync(Guid id, RouteUpdateDto dto);
        Task<IResult> DeleteAsync(Guid id);
    }
}
