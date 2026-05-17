namespace App.Business.Abstract.Services
{
    public interface IAirportService
    {
        Task<IDataResult<AirportDto>> GetByIdAsync(Guid id);
        Task<IDataResult<IEnumerable<AirportListDto>>> GetAllAsync();
        Task<IDataResult<AirportDto>> AddAsync(AirportAddDto dto);
        Task<IResult> UpdateAsync(Guid id, AirportUpdateDto dto);
        Task<IResult>                             DeleteAsync(Guid id);
        Task<IDataResult<IEnumerable<string>>>    GetDistinctCountriesAsync();
        Task<IDataResult<IEnumerable<string>>>    GetDistinctTimezonesAsync();
    }
}
