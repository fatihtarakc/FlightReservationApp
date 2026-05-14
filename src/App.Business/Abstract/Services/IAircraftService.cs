namespace App.Business.Abstract.Services
{
    public interface IAircraftService
    {
        Task<IDataResult<AircraftDto>> GetByIdAsync(Guid id);
        Task<IDataResult<IEnumerable<AircraftListDto>>> GetAllAsync();
        Task<IDataResult<AircraftDto>> AddAsync(AircraftAddDto dto);
        Task<IResult> UpdateAsync(Guid id, AircraftUpdateDto dto);
        Task<IResult> DeleteAsync(Guid id);
    }
}
