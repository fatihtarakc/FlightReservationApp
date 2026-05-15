namespace App.Web.Services.Abstract
{
    public interface IAccountWebService
    {
        Task<ApiResponse<TokenViewModel>?> SignInAsync(LoginViewModel model);
        Task<ApiResponse<object>?> RegisterAsync(RegisterViewModel model);
        Task<ApiResponse<object>?> VerifyEmailAsync(VerifyEmailViewModel model);
        Task<ApiResponse<object>?> SendVerificationCodeAsync(string email, int purpose, int channel);
        Task<ApiResponse<object>?> ResetPasswordAsync(ResetPasswordViewModel model);
    }
}
