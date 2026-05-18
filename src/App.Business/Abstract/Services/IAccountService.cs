namespace App.Business.Abstract.Services
{
    public interface IAccountService
    {
        Task<IResult> SignUpAsync(SignUpDto dto);
        Task<IDataResult<TokenDto>> SignInAsync(SignInDto dto);
        Task<IResult> SignOutAsync(string identityId);
        Task<IResult> SendVerificationCodeAsync(string email, VerificationCodePurpose purpose, VerificationCodeChannel channel);
        Task<IResult> VerifyCodeAsync(string email, string code, VerificationCodePurpose purpose);
        Task<IResult> ResetPasswordAsync(ResetPasswordDto dto);
        Task<IResult> ChangePasswordAsync(string identityId, ChangePasswordDto dto);
    }
}
