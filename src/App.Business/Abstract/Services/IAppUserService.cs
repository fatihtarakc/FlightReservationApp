namespace App.Business.Abstract.Services
{
    public interface IAppUserService
    {
        Task<IDataResult<AppUserDto>> GetByIdAsync(Guid id);
        Task<IDataResult<AppUserDto>> GetByIdentityIdAsync(string identityId);
        Task<IDataResult<IEnumerable<AppUserListDto>>> GetAllAsync();
        Task<IResult> UpdateAsync(Guid id, AppUserDto dto);
        Task<IResult> SetStatusAsync(Guid id, bool isActive);
        Task<IResult> ConfirmEmailAsync(Guid id);
    }
}
