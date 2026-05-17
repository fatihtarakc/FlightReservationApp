namespace App.Dtos.AccountDtos
{
    public class SignInDto
    {
        public string UsernameOrEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}