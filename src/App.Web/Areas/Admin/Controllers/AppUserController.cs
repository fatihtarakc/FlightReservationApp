using App.Web.Controllers;
using Microsoft.Extensions.Localization;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppUserController : BaseController
    {
        private readonly IAppUserService _appUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public AppUserController(IAppUserService appUserService,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<SharedResources> localizer)
        {
            _appUserService = appUserService;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        public async Task<IActionResult> Index(string? search)
        {
            var result = await _appUserService.GetAllAsync(Token!);
            var users = result.IsSuccess ? result.Data ?? new() : new();

            if (!string.IsNullOrWhiteSpace(search))
                users = users.Where(u =>
                    u.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Surname.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.Search = search;
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmail(Guid id)
        {
            var result = await _appUserService.ConfirmEmailAsync(id, Token!);
            if (result.IsSuccess)
                NotifySuccessLocalized(result.Message);
            else
                NotifyErrorLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(Guid id, bool isActive)
        {
            var result = await _appUserService.SetStatusAsync(id, isActive, Token!);
            if (result.IsSuccess)
                NotifySuccessLocalized(result.Message);
            else
                NotifyErrorLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
