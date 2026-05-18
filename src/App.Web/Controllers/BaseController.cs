using AspNetCoreHero.ToastNotification.Abstractions;

namespace App.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private const string ToastKey = "_sw_toasts";

        [FromServices]
        public INotyfService NotyfService { get; set; } = null!;

        private void Queue(string type, string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) return;
            var entry = System.Text.Json.JsonSerializer.Serialize(new { t = type, m = msg });
            var curr = TempData.Peek(ToastKey) as string ?? "[]";
            TempData[ToastKey] = curr.Length <= 2 ? $"[{entry}]" : curr[..^1] + "," + entry + "]";
        }

        protected void NotifySuccess(string message)         { NotyfService.Success(message);         Queue("success", message); }
        protected void NotifyError(string message)           { NotyfService.Error(message);           Queue("danger",  message); }
        protected void NotifyInfo(string message)            { NotyfService.Information(message);     Queue("info",    message); }
        protected void NotifyWarning(string message)         { NotyfService.Warning(message);         Queue("warning", message); }
        protected void NotifySuccessLocalized(string? message) { NotyfService.Success(message ?? ""); Queue("success", message ?? ""); }
        protected void NotifyErrorLocalized(string? message)   { NotyfService.Error(message ?? "");   Queue("danger",  message ?? ""); }
        protected void NotifyInfoLocalized(string? message)    { NotyfService.Information(message ?? ""); Queue("info", message ?? ""); }
        protected void NotifyWarningLocalized(string? message) { NotyfService.Warning(message ?? ""); Queue("warning", message ?? ""); }

        protected void NotifyValidationErrors()
        {
            foreach (var error in ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                         .Where(m => !string.IsNullOrWhiteSpace(m)))
            {
                NotyfService.Error(error);
                Queue("danger", error);
            }
        }
    }
}
