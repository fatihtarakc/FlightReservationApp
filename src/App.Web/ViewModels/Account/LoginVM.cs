namespace App.Web.ViewModels.Account
{
    public class LoginVM
    {
        public string UsernameOrEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
