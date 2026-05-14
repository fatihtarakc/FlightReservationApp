namespace App.Dtos.TokenDtos
{
    public record TokenDto(
        string AccessToken,
        DateTime Expiration);
}
