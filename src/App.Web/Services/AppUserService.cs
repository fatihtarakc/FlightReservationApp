namespace App.Web.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly HttpClient _http;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<AppUserService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public AppUserService(IHttpClientFactory factory, IStringLocalizer<SharedResources> localizer, ILogger<AppUserService> logger)
        {
            _http = factory.CreateClient("ApiClient");
            _localizer = localizer;
            _logger = logger;
        }

        private HttpRequestMessage Auth(HttpMethod method, string url, string token)
        {
            var req = new HttpRequestMessage(method, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return req;
        }

        public async Task<IDataResult<List<AdminUserVM>>> GetAllAsync(string token)
        {
            try
            {
                var response = await _http.SendAsync(Auth(HttpMethod.Get, "api/AppUser", token));
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<AdminUserVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<AdminUserVM>>(result.Data)
                    : new ErrorDataResult<List<AdminUserVM>>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<List<AdminUserVM>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> ConfirmEmailAsync(Guid id, string token)
        {
            try
            {
                var response = await _http.SendAsync(Auth(HttpMethod.Post, $"api/AppUser/{id}/confirm-email", token));
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(result.Message ?? _localizer[Messages.Data_LoadSuccess])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> SetStatusAsync(Guid id, bool isActive, string token)
        {
            try
            {
                var req = Auth(HttpMethod.Post, $"api/AppUser/{id}/status", token);
                req.Content = new StringContent(JsonSerializer.Serialize(new { isActive }), System.Text.Encoding.UTF8, "application/json");
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(result.Message ?? _localizer[Messages.Data_LoadSuccess])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }
    }
}
