namespace App.Queue.Services.Abstract
{
    public interface ISmsSenderService
    {
        Task<IResult> SendSmsAsync(string toPhoneNumber, string message);
        Task<IResult> SendWhatsAppAsync(string toPhoneNumber, string message);
    }
}
