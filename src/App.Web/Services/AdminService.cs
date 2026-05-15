namespace App.Web.Services
{
    public class AdminService : IAdminService
    {
        private readonly HttpClient _http;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<AdminService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public AdminService(IHttpClientFactory factory, IStringLocalizer<SharedResources> localizer, ILogger<AdminService> logger)
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

        public async Task<IDataResult<AdminDashboardVM>> GetDashboardAsync(string token)
        {
            try
            {
                var response = await _http.SendAsync(Auth(HttpMethod.Get, "api/admin/dashboard", token));
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<AdminDashboardVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<AdminDashboardVM>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<AdminDashboardVM>(result?.Message ?? _localizer[Messages.Dashboard_LoadError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<AdminDashboardVM>(message);
            }
        }

        public async Task<IDataResult<List<NotificationLogVM>>> GetNotificationLogsAsync(string token, string? search, string? channel, string? date)
        {
            try
            {
                var url = $"api/admin/notifications?search={search}&channel={channel}&date={date}";
                var response = await _http.SendAsync(Auth(HttpMethod.Get, url, token));
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<NotificationLogVM>>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessDataResult<List<NotificationLogVM>>(result.Data ?? new(), _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<NotificationLogVM>>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<NotificationLogVM>>(message);
            }
        }

        public async Task<IDataResult<List<AppLogVM>>> GetAppLogsAsync(string token, string? search, string? level, string? date)
        {
            try
            {
                var url = $"api/admin/logs?search={search}&level={level}&date={date}";
                var response = await _http.SendAsync(Auth(HttpMethod.Get, url, token));
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<AppLogVM>>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessDataResult<List<AppLogVM>>(result.Data ?? new(), _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<AppLogVM>>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<AppLogVM>>(message);
            }
        }

        public async Task<IDataResult<HangfireStatsVM>> GetHangfireStatsAsync(string token)
        {
            try
            {
                var response = await _http.SendAsync(Auth(HttpMethod.Get, "api/admin/hangfire/stats", token));
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<HangfireStatsVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<HangfireStatsVM>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<HangfireStatsVM>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<HangfireStatsVM>(message);
            }
        }
    }
}
