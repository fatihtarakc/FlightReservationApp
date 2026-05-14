using System.Text.Json.Serialization;

namespace App.Web.Models
{
    public class ApiResponse
    {
        [JsonPropertyName("isSuccess")]
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }
    }
}
