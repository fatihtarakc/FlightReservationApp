namespace App.Web.Services
{
    public class SeatService : ISeatService
    {
        private readonly HttpClient _http;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<SeatService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public SeatService(IHttpClientFactory factory, IStringLocalizer<SharedResources> localizer, ILogger<SeatService> logger)
        {
            _http = factory.CreateClient("ApiClient");
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<List<SeatVM>>> GetByFlightIdAsync(Guid flightId)
        {
            try
            {
                var body = await _http.GetStringAsync($"api/seats/flight/{flightId}");
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<SeatVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<SeatVM>>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<SeatVM>>(result?.Message ?? _localizer[Messages.Seat_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<SeatVM>>(message);
            }
        }

        public async Task<IDataResult<SeatVM>> GetByIdAsync(Guid id)
        {
            try
            {
                var body = await _http.GetStringAsync($"api/seats/{id}");
                var result = JsonSerializer.Deserialize<ApiResponseVM<SeatVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<SeatVM>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<SeatVM>(result?.Message ?? _localizer[Messages.Seat_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<SeatVM>(message);
            }
        }
    }
}
