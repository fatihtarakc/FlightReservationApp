namespace App.Web.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly HttpClient _http;
        private readonly ILogger<ScheduleService> _logger;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public ScheduleService(IHttpClientFactory factory, ILogger<ScheduleService> logger)
        {
            _http = factory.CreateClient("ApiClient");
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
    }
}
