using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Localization;

namespace App.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public AccountController(IAccountService accountService, IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<SharedResources> localizer)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        [HttpGet]
        public IActionResult SignIn(string? returnUrl = null, Guid? pendingFlightId = null, Guid? pendingSeatId = null)
        {
            if (pendingFlightId.HasValue && pendingSeatId.HasValue)
            {
                HttpContext.Session.SetString("PendingFlightId", pendingFlightId.Value.ToString());
                HttpContext.Session.SetString("PendingSeatId", pendingSeatId.Value.ToString());
            }
            return View(new SignInVM { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.SignInAsync(model);
            if (!result.IsSuccess)
            {
                if (result.Message == _localizer[Messages.Account_Email_Has_Not_Confirmed])
                {
                    var emailForVerify = model.UsernameOrEmail.Contains('@') ? model.UsernameOrEmail : string.Empty;
                    TempData["VerifyEmail_Info"] = _localizer[Messages.Account_Email_Has_Not_Confirmed].Value;
                    return RedirectToAction(nameof(VerifyEmail), new { email = emailForVerify });
                }
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

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, name),
                new(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            NotifySuccessLocalized(result.Message);

            var pendingFlightId = HttpContext.Session.GetString("PendingFlightId");
            var pendingSeatId   = HttpContext.Session.GetString("PendingSeatId");
            if (!string.IsNullOrEmpty(pendingFlightId) && !string.IsNullOrEmpty(pendingSeatId))
            {
                HttpContext.Session.Remove("PendingFlightId");
                HttpContext.Session.Remove("PendingSeatId");
                return Redirect($"/Passenger/Booking/Create?flightId={pendingFlightId}&seatId={pendingSeatId}");
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return role == "Admin"
                ? RedirectToAction("Index", "Home", new { area = "Admin" })
                : RedirectToAction("Index", "Home", new { area = "Passenger" });
        }

        [HttpPost("api/account/signin-panel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignInPanel([FromForm] SignInVM model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = _localizer["Account_SignInError"].Value });

            var result = await _accountService.SignInAsync(model);
            if (!result.IsSuccess)
                return Json(new { success = false, message = result.Message ?? _localizer["Account_SignInError"].Value });

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

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, name),
                new(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            var redirectUrl = role == "Admin"
                ? Url.Action("Index", "Home", new { area = "Admin" })
                : Url.Action("Index", "Home", new { area = "Passenger" });

            return Json(new { success = true, redirectUrl });
        }

        [HttpGet]
        public IActionResult VerifyEmail(string? email)
        {
            ViewBag.InfoMessage = TempData["VerifyEmail_Info"] as string;
            return View(new VerifyEmailVM { Email = email ?? string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _accountService.VerifyEmailAsync(model.Email, model.Code);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(nameof(model.Code), result.Message ?? "");
                return View(model);
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(SignIn));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            var result = await _accountService.SendEmailConfirmationCodeAsync(email);
            if (result.IsSuccess)
                NotifySuccessLocalized(result.Message);
            else
                NotifyErrorLocalized(result.Message);
            return RedirectToAction(nameof(VerifyEmail), new { email });
        }

        [HttpGet]
        public IActionResult SignUp() => View(new SignUpVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.SignUpAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "");
                return View(model);
            }

            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(VerifyEmail), new { email = model.Email });
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
                ModelState.AddModelError("", result.Message ?? _localizer[Messages.UnexpectedError]);
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
                ModelState.AddModelError("", result.Message ?? _localizer[Messages.UnexpectedError]);
                return View(model);
            }
            NotifySuccessLocalized(_localizer[Messages.Account_ResetPassword_Successful]);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public new async Task<IActionResult> SignOut()
        {
            var token = TokenHelper.GetToken(_httpContextAccessor);
            if (!string.IsNullOrEmpty(token))
                await _accountService.SignOutAsync(token);
            TokenHelper.ClearSession(_httpContextAccessor);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}
