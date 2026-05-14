namespace App.Business.Abstract.Services
{
    public interface IAirlineService
    {
        Task<IDataResult<AirlineDto>> GetByIdAsync(Guid id);
        Task<IDataResult<IEnumerable<AirlineListDto>>> GetAllAsync();
        Task<IDataResult<AirlineDto>> AddAsync(AirlineAddDto dto);
        Task<IResult> UpdateAsync(Guid id, AirlineUpdateDto dto);
        Task<IResult> DeleteAsync(Guid id);
    }
}
