namespace App.Business.Abstract.Services
{
    public interface ITokenService
    {
        IDataResult<TokenDto> GenerateToken(IdentityUser user, IList<string> roles);
    }
}
