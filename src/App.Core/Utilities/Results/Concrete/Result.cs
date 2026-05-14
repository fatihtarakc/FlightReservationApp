namespace App.Core.Utilities.Results.Concrete
{
    public class Result : IResult
    {
        public Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; }
        public string Message { get; }
    }
}
