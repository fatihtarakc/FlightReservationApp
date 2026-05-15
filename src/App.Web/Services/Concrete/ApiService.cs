namespace App.Web.Services.Concrete
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FlightReservationApi");
        }

        public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint, string? token = null)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync(endpoint);
            return await DeserializeAsync<T>(response);
        }

        public async Task<ApiResponse<T>?> PostAsync<T>(string endpoint, object data, string? token = null)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            return await DeserializeAsync<T>(response);
        }

        public async Task<ApiResponse<T>?> PutAsync<T>(string endpoint, object data, string? token = null)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content);
            return await DeserializeAsync<T>(response);
        }

        public async Task<ApiResponse<T>?> DeleteAsync<T>(string endpoint, string? token = null)
        {
            SetAuthHeader(token);
            var response = await _httpClient.DeleteAsync(endpoint);
            return await DeserializeAsync<T>(response);
        }

        private void SetAuthHeader(string? token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(token)
                ? new AuthenticationHeaderValue("Bearer", token)
                : null;
        }

        private static async Task<ApiResponse<T>?> DeserializeAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return new ApiResponse<T> { Success = false, Message = $"HTTP {(int)response.StatusCode}" };
            try
            {
                return JsonSerializer.Deserialize<ApiResponse<T>>(json, _jsonOptions);
            }
            catch
            {
                return new ApiResponse<T> { Success = false, Message = $"HTTP {(int)response.StatusCode}" };
            }
        }
    }
}
