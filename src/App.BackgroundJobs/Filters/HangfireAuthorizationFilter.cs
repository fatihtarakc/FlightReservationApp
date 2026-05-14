using System.Net;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace App.BackgroundJobs.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var remoteIp = httpContext.Connection.RemoteIpAddress;
            if (remoteIp != null && (IPAddress.IsLoopback(remoteIp) || remoteIp.ToString() == "::1"))
                return true;

            return httpContext.User.Identity?.IsAuthenticated == true;
        }
    }
}
