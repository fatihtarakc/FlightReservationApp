namespace App.Web.Services.Interfaces
{
    public interface IAppUserService
    {
        Task<IDataResult<List<AdminUserVM>>> GetAllAsync(string token);
        Task<IResult> ConfirmEmailAsync(Guid id, string token);
        Task<IResult> SetStatusAsync(Guid id, bool isActive, string token);
    }
}
