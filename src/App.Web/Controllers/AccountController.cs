using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace App.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IAccountService accountService, IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult SignIn(string? returnUrl = null) => View(new LoginVM { ReturnUrl = returnUrl });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.LoginAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "");
                return View(model);
            }

            var token = result.Data!.AccessToken;
            TokenHelper.SetToken(_httpContextAccessor, token);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
                       ?? jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "AppUser";
            var name = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                       ?? jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "";

            TokenHelper.SetRole(_httpContextAccessor, role);
            TokenHelper.SetUserName(_httpContextAccessor, name);

            NotifySuccessLocalized(result.Message);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return role == "Admin"
                ? RedirectToAction("Index", "Home", new { area = "Admin" })
                : RedirectToAction("Index", "Home", new { area = "Passenger" });
        }

        [HttpGet]
        public IActionResult SignUp() => View(new RegisterVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.RegisterAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "");
                return View(model);
            }

            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View(new ForgotPasswordVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _accountService.ForgotPasswordAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "Hata.");
                return View(model);
            }
            NotifyInfoLocalized(result.Message);
            return RedirectToAction(nameof(ResetPassword), new { email = model.Email });
        }

        [HttpGet]
        public IActionResult ResetPassword(string? email)
        {
            return View(new ResetPasswordVM { Email = email ?? string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _accountService.ResetPasswordAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "İşlem başarısız.");
                return View(model);
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(SignIn));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var token = TokenHelper.GetToken(_httpContextAccessor);
            if (!string.IsNullOrEmpty(token))
                await _accountService.LogoutAsync(token);
            TokenHelper.ClearSession(_httpContextAccessor);
            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}
