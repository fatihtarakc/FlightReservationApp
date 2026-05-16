using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IAdminService _adminService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IAdminService adminService, IHttpContextAccessor httpContextAccessor)
        {
            _adminService = adminService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        private IActionResult RedirectToLogin() =>
            RedirectToAction("SignIn", "Account", new { area = "" });

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(Token)) return RedirectToLogin();
            var result = await _adminService.GetDashboardAsync(Token);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            return View(result.IsSuccess ? result.Data ?? new AdminDashboardVM() : new AdminDashboardVM());
        }

        public async Task<IActionResult> Notifications(string? search, string? channel, string? date)
        {
            if (string.IsNullOrEmpty(Token)) return RedirectToLogin();
            var result = await _adminService.GetNotificationLogsAsync(Token, search, channel, date);
            var vm = new AdminNotificationListPageVM
            {
                Notifications = result.IsSuccess ? result.Data ?? new() : new(),
                Search = search,
                Channel = channel,
                DateFilter = date
            };
            return View(vm);
        }

        public async Task<IActionResult> Logs(string? search, string? level, string? date)
        {
            if (string.IsNullOrEmpty(Token)) return RedirectToLogin();
            var result = await _adminService.GetAppLogsAsync(Token, search, level, date);
            var vm = new AdminLogListPageVM
            {
                Logs = result.IsSuccess ? result.Data ?? new() : new(),
                Search = search,
                Level = level,
                DateFilter = date
            };
            return View(vm);
        }

        public async Task<IActionResult> Hangfire()
        {
            if (string.IsNullOrEmpty(Token)) return RedirectToLogin();
            var result = await _adminService.GetHangfireStatsAsync(Token);
            var vm = new AdminHangfirePageVM
            {
                Stats = result.IsSuccess && result.Data != null ? result.Data : new HangfireStatsVM()
            };
            return View(vm);
        }
    }
}
