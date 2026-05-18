namespace App.Web.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly HttpClient _http;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<ScheduleService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public ScheduleService(IHttpClientFactory factory, IStringLocalizer<SharedResources> localizer, ILogger<ScheduleService> logger)
        {
            _http = factory.CreateClient("ApiClient");
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<List<ScheduleSelectItemVM>> GetAllAsync()
        {
            try
            {
                var body = await _http.GetStringAsync("api/Schedule");
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<ScheduleSelectItemVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null ? result.Data : new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Schedule list could not be loaded.");
                return new();
            }
        }

        public async Task<IDataResult<List<ScheduleVM>>> GetAllFullAsync()
        {
            try
            {
                var body = await _http.GetStringAsync("api/Schedule");
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<ScheduleVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<ScheduleVM>>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<ScheduleVM>>(result?.Message ?? _localizer[Messages.Schedule_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<ScheduleVM>>(message);
            }
        }

        public async Task<IDataResult<ScheduleVM>> GetByIdAsync(Guid id)
        {
            try
            {
                var body = await _http.GetStringAsync($"api/Schedule/{id}");
                var result = JsonSerializer.Deserialize<ApiResponseVM<ScheduleVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<ScheduleVM>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<ScheduleVM>(result?.Message ?? _localizer[Messages.Schedule_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<ScheduleVM>(message);
            }
        }

        public async Task<IDataResult<ScheduleVM>> AddAsync(ScheduleAddVM model, string token)
        {
            try
            {
                var days = (DaysOfWeek)model.SelectedDays.Aggregate(0, (acc, d) => acc | d);
                var payload = new
                {
                    Code          = model.Code,
                    ValidFrom     = model.ValidFrom,
                    ValidTo       = model.ValidTo,
                    DaysOfWeek    = days,
                    DepartureTime = TimeSpan.Parse(model.DepartureTime),
                    TimeZone      = model.TimeZone,
                    RouteId       = model.RouteId
                };
                var req = new HttpRequestMessage(HttpMethod.Post, "api/Schedule")
                { Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<ScheduleVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<ScheduleVM>(result.Data, _localizer[Messages.Schedule_HasBeen_Added])
                    : new ErrorDataResult<ScheduleVM>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<ScheduleVM>(message);
            }
        }

        public async Task<IResult> UpdateAsync(Guid id, ScheduleUpdateVM model, string token)
        {
            try
            {
                var days = (DaysOfWeek)model.SelectedDays.Aggregate(0, (acc, d) => acc | d);
                var payload = new
                {
                    ValidFrom     = model.ValidFrom,
                    ValidTo       = model.ValidTo,
                    DaysOfWeek    = days,
                    DepartureTime = TimeSpan.Parse(model.DepartureTime),
                    TimeZone      = model.TimeZone
                };
                var req = new HttpRequestMessage(HttpMethod.Put, $"api/Schedule/{id}")
                { Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Schedule_Was_Updated])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<bool> HasFlightsAsync(Guid id)
        {
            try
            {
                var body = await _http.GetStringAsync($"api/Schedule/{id}/has-flights");
                var result = JsonSerializer.Deserialize<ApiResponseVM<bool>>(body, _opts);
                return result?.IsSuccess == true && result.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HasFlights check failed for schedule {Id}", id);
                return false;
            }
        }

        public async Task<IResult> DeleteAsync(Guid id, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Delete, $"api/Schedule/{id}");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Schedule_Was_Deleted])
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
