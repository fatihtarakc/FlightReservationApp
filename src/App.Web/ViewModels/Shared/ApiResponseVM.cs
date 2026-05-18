namespace App.Web.ViewModels.Shared
{
    public class HomePageVM { }

    public class ApiResponseVM<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
