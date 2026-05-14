using CoreTokenOptions = App.Core.Options.TokenOptions;

namespace App.Business.Concrete.Services
{
    public class TokenService : ITokenService
    {
        private readonly CoreTokenOptions _tokenOptions;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IOptions<CoreTokenOptions> tokenOptions,
            IStringLocalizer<MessageResources> localizer,
            ILogger<TokenService> logger)
        {
            _tokenOptions = tokenOptions.Value;
            _localizer = localizer;
            _logger = logger;
        }

        public IDataResult<TokenDto> GenerateToken(IdentityUser user, IList<string> roles)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

                var keyBytes = Encoding.UTF8.GetBytes(_tokenOptions.IssuerSigningSymmetricSecurityKey);
                var securityKey = new SymmetricSecurityKey(keyBytes);
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var expiration = DateTime.UtcNow.AddMinutes(_tokenOptions.Expiration);

                var token = new JwtSecurityToken(
                    issuer: _tokenOptions.Issuer,
                    audience: _tokenOptions.Audience,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: expiration,
                    signingCredentials: credentials);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("{Message} UserId: {UserId}", _localizer[Messages.Token_Was_Generated_Successfully].Value, user.Id);
                return new SuccessDataResult<TokenDto>(
                    new TokenDto(tokenString, expiration),
                    _localizer[Messages.Token_Was_Generated_Successfully]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", _localizer[Messages.Token_Could_Not_Generated].Value);
                return new ErrorDataResult<TokenDto>(_localizer[Messages.Token_Could_Not_Generated]);
            }
        }
    }
}
