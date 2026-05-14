namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IAppUserRepository :
        IAsyncAddableRepository<AppUser>, IAsyncDeletableRepository<AppUser>,
        IAsyncUpdatableRepository<AppUser>, IAsyncQueryableRepository<AppUser>,
        IAsyncOrderableRepository<AppUser>
    {
        Task<AppUser> GetByIdentityIdAsync(string identityId, bool tracking = true);
        Task<AppUser> GetByEmailAsync(string email, bool tracking = true);
    }
}
