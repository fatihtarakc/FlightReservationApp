using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IAccountService accountService, IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        public async Task<IActionResult> Index(string? search, string? role, bool? active)
        {
            var result = await _accountService.GetAllUsersAsync(Token!);
            var users = result.IsSuccess ? result.Data ?? new() : new();

            if (!string.IsNullOrWhiteSpace(search))
                users = users.Where(u =>
                    u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(role))
                users = users.Where(u => u.Role == role).ToList();

            if (active.HasValue)
                users = users.Where(u => u.IsActive == active.Value).ToList();

            return View(new AdminUserListPageVM
            {
                Users = users,
                Search = search,
                RoleFilter = role,
                ActiveFilter = active
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _accountService.GetAllUsersAsync(Token!);
            if (!result.IsSuccess) { NotifyErrorLocalized(result.Message); return RedirectToAction(nameof(Index)); }
            var user = result.Data?.FirstOrDefault(u => u.Id == id);
            if (user == null) { NotifyError("Kullanıcı bulunamadı."); return RedirectToAction(nameof(Index)); }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(Guid id)
        {
            var result = await _accountService.SetUserStatusAsync(id, true, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var result = await _accountService.SetUserStatusAsync(id, false, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
