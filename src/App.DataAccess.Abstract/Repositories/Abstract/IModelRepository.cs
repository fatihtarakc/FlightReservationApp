namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IModelRepository :
        IAsyncAddableRepository<Model>, IAsyncDeletableRepository<Model>,
        IAsyncUpdatableRepository<Model>, IAsyncQueryableRepository<Model>,
        IAsyncOrderableRepository<Model>
    {
        Task<IEnumerable<Model>> GetByManufacturerIdAsync(Guid manufacturerId, bool tracking = true);
    }
}
