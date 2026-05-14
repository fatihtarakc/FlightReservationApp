namespace App.DataAccess.Abstract.Repositories.Abstract
{
    public interface IVerificationCodeRepository :
        IAsyncAddableRepository<VerificationCode>, IAsyncDeletableRepository<VerificationCode>,
        IAsyncUpdatableRepository<VerificationCode>, IAsyncQueryableRepository<VerificationCode>,
        IAsyncOrderableRepository<VerificationCode>
    {
        Task<VerificationCode> GetActiveCodeAsync(Guid userId, App.Entity.Enums.VerificationCodePurpose purpose, bool tracking = true);
    }
}
