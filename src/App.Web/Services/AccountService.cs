using System.IdentityModel.Tokens.Jwt;
namespace App.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _http;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<AccountService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public AccountService(IHttpClientFactory factory, IStringLocalizer<SharedResources> localizer, ILogger<AccountService> logger)
        {
            _http = factory.CreateClient("ApiClient");
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<TokenResponseVM>> LoginAsync(LoginVM model)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/login")
                { Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json") };
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<TokenResponseVM>>(body, _opts);
                if (result?.IsSuccess == true && result.Data != null)
                    return new SuccessDataResult<TokenResponseVM>(result.Data, _localizer[Messages.Account_SignIn_Successful]);
                return new ErrorDataResult<TokenResponseVM>(result?.Message ?? _localizer[Messages.Account_SignIn_Failed]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<TokenResponseVM>(message);
            }
        }

        public async Task<IResult> RegisterAsync(RegisterVM model)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/register")
                { Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json") };
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.AppUser_HasBeen_Added])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.AppUser_CouldNotBe_Added]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordVM model)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/forgot-password")
                { Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json") };
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Account_ForgotPassword_Successful])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordVM model)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/reset-password")
                { Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json") };
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Account_ResetPassword_Successful])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<IResult> LogoutAsync(string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await _http.SendAsync(req);
                return new SuccessResult(_localizer[Messages.Account_SignOut_Successful]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<IDataResult<PassengerProfileVM>> GetProfileAsync(string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, "api/account/profile");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<PassengerProfileVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<PassengerProfileVM>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<PassengerProfileVM>(result?.Message ?? _localizer[Messages.Account_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<PassengerProfileVM>(message);
            }
        }

        public async Task<IResult> UpdateProfileAsync(PassengerProfileUpdateVM model, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Put, "api/account/profile")
                { Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Profile_UpdateSuccess])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<IResult> ChangePasswordAsync(ChangePasswordVM model, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "api/account/change-password")
                { Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Password_ChangeSuccess])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<IResult> UpdateNotificationPreferenceAsync(NotificationChannel preference, string token)
        {
            try
            {
                var payload = new { NotificationPreference = (int)preference };
                var req = new HttpRequestMessage(HttpMethod.Put, "api/account/notification-preference")
                { Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Notif_UpdateSuccess])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<IDataResult<List<AdminUserVM>>> GetAllUsersAsync(string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, "api/admin/users");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<AdminUserVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<AdminUserVM>>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<AdminUserVM>>(result?.Message ?? _localizer[Messages.AppUser_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<AdminUserVM>>(message);
            }
        }

        public async Task<IResult> SetUserStatusAsync(Guid id, bool isActive, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, $"api/admin/users/{id}/status")
                { Content = new StringContent(JsonSerializer.Serialize(new { isActive }), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                var successMsg = isActive ? _localizer[Messages.AppUser_Was_Activated] : _localizer[Messages.AppUser_Was_Deactivated];
                return result?.IsSuccess == true
                    ? new SuccessResult(successMsg)
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }
    }
}
