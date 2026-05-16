namespace App.Web.ViewModels.Account
{
    public class SignInVM
    {
        public string UsernameOrEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
