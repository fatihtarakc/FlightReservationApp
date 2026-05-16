using System.Globalization;

namespace App.Web.Helpers
{
    public class CultureDelegatingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.ParseAdd(CultureInfo.CurrentUICulture.Name);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
