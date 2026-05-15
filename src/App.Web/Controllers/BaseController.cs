using AspNetCoreHero.ToastNotification.Abstractions;

namespace App.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        [FromServices]
        public INotyfService NotyfService { get; set; } = null!;

        protected void NotifySuccess(string message) => NotyfService.Success(message);
        protected void NotifyError(string message) => NotyfService.Error(message);
        protected void NotifyInfo(string message) => NotyfService.Information(message);
        protected void NotifyWarning(string message) => NotyfService.Warning(message);
        protected void NotifySuccessLocalized(string? message) => NotyfService.Success(message ?? string.Empty);
        protected void NotifyErrorLocalized(string? message) => NotyfService.Error(message ?? string.Empty);
        protected void NotifyInfoLocalized(string? message) => NotyfService.Information(message ?? string.Empty);
        protected void NotifyWarningLocalized(string? message) => NotyfService.Warning(message ?? string.Empty);
    }
}
