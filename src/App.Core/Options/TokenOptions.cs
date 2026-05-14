namespace App.Core.Options
{
    public class TokenOptions
    {
        public const string TokenConfiguration = "TokenConfiguration";

        public string Audience { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string IssuerSigningSymmetricSecurityKey { get; set; } = null!;
        public int Expiration { get; set; }
    }
}
