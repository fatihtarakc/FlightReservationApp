namespace App.Business.Abstract.Services
{
    public interface IVerificationCodeService
    {
        Task<IDataResult<string>> GenerateAndSaveAsync(Guid userId, VerificationCodePurpose purpose, VerificationCodeChannel channel);
        Task<IResult> ValidateAsync(Guid userId, string code, VerificationCodePurpose purpose);
    }
}
