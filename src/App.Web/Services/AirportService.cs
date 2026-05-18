namespace App.Web.Services
{
    public class AirportService : IAirportService
    {
        private readonly HttpClient _http;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<AirportService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public AirportService(IHttpClientFactory factory, IStringLocalizer<SharedResources> localizer, ILogger<AirportService> logger)
        {
            _http = factory.CreateClient("ApiClient");
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<List<AirportVM>>> GetAllAsync()
        {
            try
            {
                var body = await _http.GetStringAsync("api/Airport");
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<AirportVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<AirportVM>>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<AirportVM>>(result?.Message ?? _localizer[Messages.Airport_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<AirportVM>>(message);
            }
        }

        public async Task<IDataResult<AirportVM>> GetByIdAsync(Guid id)
        {
            try
            {
                var body = await _http.GetStringAsync($"api/Airport/{id}");
                var result = JsonSerializer.Deserialize<ApiResponseVM<AirportVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<AirportVM>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<AirportVM>(result?.Message ?? _localizer[Messages.Airport_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<AirportVM>(message);
            }
        }

        public async Task<IDataResult<AirportVM>> AddAsync(AirportAddVM model, string token)
        {
            try
            {
                var payload = new
                {
                    model.Name, model.IataCode, model.IcaoCode,
                    model.City, model.Country,
                    TimeZone  = model.Timezone,
                    model.Latitude, model.Longitude
                };
                var req = new HttpRequestMessage(HttpMethod.Post, "api/Airport")
                { Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<AirportVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<AirportVM>(result.Data, _localizer[Messages.Airport_HasBeen_Added])
                    : new ErrorDataResult<AirportVM>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<AirportVM>(message);
            }
        }

        public async Task<IDataResult<AirportVM>> UpdateAsync(Guid id, AirportAddVM model, string token)
        {
            try
            {
                var payload = new
                {
                    model.Name, model.IataCode, model.IcaoCode,
                    model.City, model.Country,
                    TimeZone  = model.Timezone,
                    model.Latitude, model.Longitude
                };
                var req = new HttpRequestMessage(HttpMethod.Put, $"api/Airport/{id}")
                { Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<AirportVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<AirportVM>(result.Data, _localizer[Messages.Airport_Was_Updated])
                    : new ErrorDataResult<AirportVM>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<AirportVM>(message);
            }
        }

        public async Task<IResult> DeleteAsync(Guid id, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Delete, $"api/Airport/{id}");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Airport_Was_Deleted])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<List<string>> GetCountriesAsync()
        {
            try
            {
                var body = await _http.GetStringAsync("api/Airport/countries");
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<string>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null ? result.Data : new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new List<string>();
            }
        }

        public async Task<List<string>> GetTimezonesAsync()
        {
            try
            {
                var body = await _http.GetStringAsync("api/Airport/timezones");
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<string>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null ? result.Data : new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new List<string>();
            }
        }
    }
}
