namespace App.Web.Services
{
    public class AircraftService : IAircraftService
    {
        private readonly HttpClient _http;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<AircraftService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public AircraftService(IHttpClientFactory factory, IStringLocalizer<SharedResources> localizer, ILogger<AircraftService> logger)
        {
            _http = factory.CreateClient("ApiClient");
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<List<AircraftVM>>> GetAllAsync()
        {
            try
            {
                var body = await _http.GetStringAsync("api/Aircraft");
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<AircraftVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<AircraftVM>>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<AircraftVM>>(result?.Message ?? _localizer[Messages.Aircraft_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<AircraftVM>>(message);
            }
        }

        public async Task<IDataResult<AircraftVM>> GetByIdAsync(Guid id)
        {
            try
            {
                var body = await _http.GetStringAsync($"api/Aircraft/{id}");
                var result = JsonSerializer.Deserialize<ApiResponseVM<AircraftVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<AircraftVM>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<AircraftVM>(result?.Message ?? _localizer[Messages.Aircraft_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<AircraftVM>(message);
            }
        }

        public async Task<IDataResult<AircraftVM>> AddAsync(AircraftAddVM model, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "api/Aircraft")
                { Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<AircraftVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<AircraftVM>(result.Data, _localizer[Messages.Aircraft_HasBeen_Added])
                    : new ErrorDataResult<AircraftVM>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<AircraftVM>(message);
            }
        }

        public async Task<IResult> UpdateAsync(Guid id, AircraftUpdateVM model, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Put, $"api/Aircraft/{id}")
                { Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Aircraft_Was_Updated])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<List<AircraftSelectItemVM>> GetAirlinesAsync()
        {
            try
            {
                var body = await _http.GetStringAsync("api/Airline");
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<AircraftSelectItemVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null ? result.Data : new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new();
            }
        }

        public async Task<List<AircraftSelectItemVM>> GetModelsAsync()
        {
            try
            {
                var body = await _http.GetStringAsync("api/Model");
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<AircraftSelectItemVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null ? result.Data : new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new();
            }
        }

        public async Task<IResult> DeleteAsync(Guid id, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Delete, $"api/Aircraft/{id}");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Aircraft_Was_Deleted])
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
