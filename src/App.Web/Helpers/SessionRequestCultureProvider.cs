using Microsoft.AspNetCore.Localization;
namespace App.Web.Helpers
{
    public class SessionRequestCultureProvider : IRequestCultureProvider
    {
        public const string SessionKey = "AppCulture";

        public Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var culture = httpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(culture))
                return Task.FromResult<ProviderCultureResult?>(null);

            return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(culture));
        }
    }
}
