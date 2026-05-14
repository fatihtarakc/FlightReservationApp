namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IManufacturerRepository :
        IAsyncAddableRepository<Manufacturer>, IAsyncDeletableRepository<Manufacturer>,
        IAsyncUpdatableRepository<Manufacturer>, IAsyncQueryableRepository<Manufacturer>,
        IAsyncOrderableRepository<Manufacturer>
    {
    }
}
