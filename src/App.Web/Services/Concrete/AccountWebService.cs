namespace App.Web.Services.Concrete
{
    public class AccountWebService : IAccountWebService
    {
        private readonly IApiHttpClient _api;

        public AccountWebService(IApiHttpClient api)
        {
            _api = api;
        }

        public Task<ApiResponse<TokenViewModel>?> SignInAsync(LoginViewModel model) =>
            _api.PostAsync<TokenViewModel>("account/sign-in", model);

        public Task<ApiResponse<object>?> RegisterAsync(RegisterViewModel model) =>
            _api.PostAsync<object>("account/register", model);

        public Task<ApiResponse<object>?> VerifyEmailAsync(VerifyEmailViewModel model) =>
            _api.PostAsync<object>("account/verify-code", model);

        public Task<ApiResponse<object>?> SendVerificationCodeAsync(string email, int purpose, int channel) =>
            _api.PostAsync<object>($"account/send-verification-code?email={Uri.EscapeDataString(email)}&purpose={purpose}&channel={channel}", new { });

        public Task<ApiResponse<object>?> ResetPasswordAsync(ResetPasswordViewModel model) =>
            _api.PostAsync<object>("account/reset-password", model);
    }
}
