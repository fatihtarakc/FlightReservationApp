namespace App.Web.Services
{
    public class RouteService : IRouteService
    {
        private readonly HttpClient _http;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<RouteService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public RouteService(IHttpClientFactory factory, IStringLocalizer<SharedResources> localizer, ILogger<RouteService> logger)
        {
            _http = factory.CreateClient("ApiClient");
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<List<RouteVM>>> GetAllAsync()
        {
            try
            {
                var body = await _http.GetStringAsync("api/Route");
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<RouteVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<RouteVM>>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<RouteVM>>(result?.Message ?? _localizer[Messages.Route_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<RouteVM>>(message);
            }
        }

        public async Task<IDataResult<RouteVM>> GetByIdAsync(Guid id)
        {
            try
            {
                var body = await _http.GetStringAsync($"api/Route/{id}");
                var result = JsonSerializer.Deserialize<ApiResponseVM<RouteVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<RouteVM>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<RouteVM>(result?.Message ?? _localizer[Messages.Route_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<RouteVM>(message);
            }
        }

        public async Task<IDataResult<RouteVM>> AddAsync(RouteAddVM model, string token)
        {
            try
            {
                var payload = new
                {
                    DepartureAirportId = model.OriginAirportId,
                    ArrivalAirportId   = model.DestinationAirportId,
                    DistanceKm         = model.DistanceKm,
                    EstimatedDuration  = TimeSpan.FromMinutes(model.EstimatedDuration)
                };
                var req = new HttpRequestMessage(HttpMethod.Post, "api/Route")
                { Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<RouteVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<RouteVM>(result.Data, _localizer[Messages.Route_HasBeen_Added])
                    : new ErrorDataResult<RouteVM>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<RouteVM>(message);
            }
        }

        public async Task<IResult> UpdateAsync(Guid id, RouteUpdateVM model, string token)
        {
            try
            {
                var payload = new
                {
                    DistanceKm        = model.DistanceKm,
                    EstimatedDuration = TimeSpan.FromMinutes(model.EstimatedDuration)
                };
                var req = new HttpRequestMessage(HttpMethod.Put, $"api/Route/{id}")
                { Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Route_Was_Updated])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<IResult> DeleteAsync(Guid id, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Delete, $"api/Route/{id}");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Route_Was_Deleted])
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
