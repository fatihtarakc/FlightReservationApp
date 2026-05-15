namespace App.Web.Utilities.Results.Concrete
{
    public class Result : Abstract.IResult
    {
        public Result(bool isSuccess, string message) { IsSuccess = isSuccess; Message = message; }
        public bool IsSuccess { get; }
        public string Message { get; }
    }
}
