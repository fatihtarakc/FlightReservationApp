namespace App.Business.Abstract.Services
{
    public interface IModelService
    {
        Task<IDataResult<ModelDto>> GetByIdAsync(Guid id);
        Task<IDataResult<IEnumerable<ModelDto>>> GetAllAsync();
        Task<IDataResult<IEnumerable<ModelDto>>> GetByManufacturerIdAsync(Guid manufacturerId);
        Task<IDataResult<ModelDto>> AddAsync(ModelAddDto dto);
        Task<IResult> UpdateAsync(Guid id, ModelUpdateDto dto);
        Task<IResult> DeleteAsync(Guid id);
    }
}
