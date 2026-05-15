namespace App.Business.Abstract.Services
{
    public interface IManufacturerService
    {
        Task<IDataResult<ManufacturerDto>> GetByIdAsync(Guid id);
        Task<IDataResult<IEnumerable<ManufacturerDto>>> GetAllAsync();
        Task<IDataResult<ManufacturerDto>> AddAsync(ManufacturerAddDto dto);
        Task<IResult> UpdateAsync(Guid id, ManufacturerUpdateDto dto);
        Task<IResult> DeleteAsync(Guid id);
    }
}
