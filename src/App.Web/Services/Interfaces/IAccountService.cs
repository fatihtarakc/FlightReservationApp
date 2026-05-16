namespace App.Web.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IDataResult<TokenResponseVM>> SignInAsync(LoginVM model);
        Task<IResult> SignUpAsync(RegisterVM model);
        Task<IResult> ForgotPasswordAsync(ForgotPasswordVM model);
        Task<IResult> ResetPasswordAsync(ResetPasswordVM model);
        Task<IResult> SignOutAsync(string token);
        Task<IDataResult<PassengerProfileVM>> GetProfileAsync(string token);
        Task<IResult> UpdateProfileAsync(PassengerProfileUpdateVM model, string token);
        Task<IResult> ChangePasswordAsync(ChangePasswordVM model, string token);
        Task<IResult> UpdateNotificationPreferenceAsync(NotificationChannel preference, string token);
        Task<IDataResult<List<AdminUserVM>>> GetAllUsersAsync(string token);
        Task<IResult> SetUserStatusAsync(Guid id, bool isActive, string token);
    }
}
