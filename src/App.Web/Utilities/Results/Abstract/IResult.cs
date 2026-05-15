namespace App.Web.Utilities.Results.Abstract
{
    public interface IResult
    {
        bool IsSuccess { get; }
        string Message { get; }
    }
}
