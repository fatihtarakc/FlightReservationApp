namespace App.Web.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _http;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<BookingService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public BookingService(IHttpClientFactory factory, IStringLocalizer<SharedResources> localizer, ILogger<BookingService> logger)
        {
            _http = factory.CreateClient("ApiClient");
            _localizer = localizer;
            _logger = logger;
        }

        private HttpRequestMessage AuthorizedGet(string url, string token)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return req;
        }

        public async Task<IDataResult<List<BookingVM>>> GetAllAsync(string token)
        {
            try
            {
                var response = await _http.SendAsync(AuthorizedGet("api/bookings", token));
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<BookingVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<BookingVM>>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<BookingVM>>(result?.Message ?? _localizer[Messages.Booking_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<BookingVM>>(message);
            }
        }

        public async Task<IDataResult<List<BookingVM>>> GetMyBookingsAsync(string token)
        {
            try
            {
                var response = await _http.SendAsync(AuthorizedGet("api/bookings/my", token));
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<List<BookingVM>>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<List<BookingVM>>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<List<BookingVM>>(result?.Message ?? _localizer[Messages.Booking_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<List<BookingVM>>(message);
            }
        }

        public async Task<IDataResult<BookingVM>> GetByIdAsync(Guid id, string token)
        {
            try
            {
                var response = await _http.SendAsync(AuthorizedGet($"api/bookings/{id}", token));
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<BookingVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<BookingVM>(result.Data, _localizer[Messages.Data_LoadSuccess])
                    : new ErrorDataResult<BookingVM>(result?.Message ?? _localizer[Messages.Booking_Was_Not_Found]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<BookingVM>(message);
            }
        }

        public async Task<IDataResult<BookingVM>> CreateAsync(BookingAddVM model, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "api/bookings")
                { Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<BookingVM>>(body, _opts);
                return result?.IsSuccess == true && result.Data != null
                    ? new SuccessDataResult<BookingVM>(result.Data, _localizer[Messages.Booking_HasBeen_Added])
                    : new ErrorDataResult<BookingVM>(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorDataResult<BookingVM>(message);
            }
        }

        public async Task<IResult> CancelAsync(Guid id, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, $"api/bookings/{id}/cancel");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Booking_Was_Cancelled])
                    : new ErrorResult(result?.Message ?? _localizer[Messages.UnexpectedError]);
            }
            catch (Exception ex)
            {
                var message = _localizer[Messages.UnexpectedError];
                _logger.LogError(ex, message);
                return new ErrorResult(message);
            }
        }

        public async Task<IResult> CheckInAsync(Guid id, string token)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, $"api/bookings/{id}/checkin");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.SendAsync(req);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponseVM<object>>(body, _opts);
                return result?.IsSuccess == true
                    ? new SuccessResult(_localizer[Messages.Booking_CheckedIn_Successfully])
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
