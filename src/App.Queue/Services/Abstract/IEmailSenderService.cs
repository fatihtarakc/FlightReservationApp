namespace App.Queue.Services.Abstract
{
    public interface IEmailSenderService
    {
        Task<IResult> SendAsync(EmailDto emailDto);
    }
}
