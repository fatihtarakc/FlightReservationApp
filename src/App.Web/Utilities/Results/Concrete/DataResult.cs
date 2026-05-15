namespace App.Web.Utilities.Results.Concrete
{
    public class DataResult<T> : Result, Abstract.IDataResult<T>
    {
        public DataResult(T data, bool isSuccess, string message) : base(isSuccess, message) { Data = data; }
        public T Data { get; }
    }
}
