namespace App.Web.ViewModels.Account
{
    public class TokenResponseVM
    {
        public string AccessToken { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
