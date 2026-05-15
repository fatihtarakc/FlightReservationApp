namespace App.Web.ViewModels.Token
{
    public class TokenViewModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
