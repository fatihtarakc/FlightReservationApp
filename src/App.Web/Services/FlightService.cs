namespace App.Web.Services
{
    public class FlightService : IFlightService
    {
        private readonly HttpClient _http;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<FlightService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public FlightService(IHttpClientFactory factory, IStringLocalizer<SharedResources> localizer, ILogger<FlightService> logger)
        {
            _http = factory.CreateClient("ApiClient");
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<List<FlightVM>>> GetAllAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/Flight");
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<FlightVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<FlightVM>>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<FlightVM>>(result?.Message ?? _localizer[Messages.Flight_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<FlightVM>>(message);
            }
        }

        public async Task<IDataResult<FlightVM>> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _http.GetAsync($"api/Flight/{id}");
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<FlightVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<FlightVM>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<FlightVM>(result?.Message ?? _localizer[Messages.Flight_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<FlightVM>(message);
            }
        }

        public async Task<IDataResult<List<FlightVM>>> SearchAsync(FlightSearchVM model)
        {
            try
            {
                var url = $"api/Flight/search?DepartureIata={model.DepartureIata}&ArrivalIata={model.ArrivalIata}&DepartureDate={model.DepartureDate:yyyy-MM-dd}&Passengers={model.Passengers}&SeatClass={(int)model.SeatClass}";
                var response = await _http.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<FlightVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<FlightVM>>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<FlightVM>>(result?.Message ?? _localizer[Messages.Flight_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<FlightVM>>(message);
            }
        }

        public async Task<IDataResult<FlightDetailPageVM>> GetDetailWithSeatsAsync(Guid id)
        {
            try
            {
                var flightTask = _http.GetAsync($"api/Flight/{id}");
                var seatsTask = _http.GetAsync($"api/Seat/flight/{id}");
                await Task.WhenAll(flightTask, seatsTask);

                var flightBody = await (await flightTask).Content.ReadAsStringAsync();
                var seatsBody = await (await seatsTask).Content.ReadAsStringAsync();

                var flightResult = JsonSerializer.Deserialize<ApiResponseVM<FlightVM>>(flightBody, _opts);
                var seatsResult = JsonSerializer.Deserialize<ApiResponseVM<List<SeatVM>>>(seatsBody, _opts);

                if (flightResult?.IsSuccess != true || flightResult.Data == null)
                    return new ErrorDataResult<FlightDetailPageVM>(_localizer[Messages.Flight_Was_Not_Found]);

                return new SuccessDataResult<FlightDetailPageVM>(new FlightDetailPageVM
                {
                    Flight = flightResult.Data,
                    Seats = seatsResult?.Data ?? new()
                }, _localizer[Messages.Data_LoadSuccess]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<FlightDetailPageVM>(message);
            }
        }

        public async Task<IDataResult<FlightVM>> AddAsync(FlightAddVM model, string token)
        {
            try
            {
                var payload = new
                {
                    Number                  = model.FlightNumber,
                    DepartureDateTime       = model.DepartureTime,
                    ArrivalDateTime         = model.ArrivalTime,
                    BaseEconomyPrice        = model.EconomyPrice,
                    BasePremiumEconomyPrice = model.PremiumEconomyPrice,
                    BaseBusinessPrice       = model.BusinessPrice,
                    BaseFirstClassPrice     = model.FirstClassPrice,
                    Currency                = model.Currency,
                    Gate                    = model.Gate,
                    Terminal                = model.Terminal,
                    AircraftId              = model.AircraftId,
                    AirlineId               = model.AirlineId,
                    ScheduleId              = model.ScheduleId
                };
                var req = new HttpRequestMessage(HttpMethod.Post, "api/Flight")
                {
                    Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<FlightVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<FlightVM>(result.Data, _localizer[Messages.Flight_HasBeen_Added])
                    : new ErrorDataResult<FlightVM>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<FlightVM>(message);
            }
        }

        public async Task<IDataResult<FlightVM>> UpdateAsync(Guid id, FlightAddVM model, string token)
        {
            try
            {
                var payload = new
                {
                    BaseEconomyPrice        = model.EconomyPrice,
                    BasePremiumEconomyPrice = model.PremiumEconomyPrice ?? 0m,
                    BaseBusinessPrice       = model.BusinessPrice ?? 0m,
                    BaseFirstClassPrice     = model.FirstClassPrice ?? 0m,
                    FlightStatus            = FlightStatus.Scheduled,
                    Gate                    = model.Gate,
                    Terminal                = model.Terminal
                };
                var req = new HttpRequestMessage(HttpMethod.Put, $"api/Flight/{id}")
                {
                    Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<FlightVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<FlightVM>(result.Data, _localizer[Messages.Flight_Was_Updated])
                    : new ErrorDataResult<FlightVM>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<FlightVM>(message);
            }
        }

        public async Task<IResult> CancelAsync(Guid id, string? reason, string token)
        {
            try
            {
                var url = string.IsNullOrEmpty(reason)
                    ? $"api/Flight/{id}/cancel"
                    : $"api/Flight/{id}/cancel?reason={Uri.EscapeDataString(reason)}";
                var req = new HttpRequestMessage(HttpMethod.Post, url);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Flight_Was_Cancelled])
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
                var req = new HttpRequestMessage(HttpMethod.Delete, $"api/Flight/{id}");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Flight_Was_Deleted])
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
