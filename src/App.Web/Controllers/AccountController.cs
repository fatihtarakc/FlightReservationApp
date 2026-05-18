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
                if (result.Message == _localizer[Messages.Account_Email_Has_Not_Confirmed].Value)
                {
                    var infoResult = await _accountService.GetVerificationInfoAsync(model.UsernameOrEmail);
                    var info = infoResult.Data;
                    var emailForVerify = info?.Email ?? string.Empty;
                    var maskedPhone = info?.MaskedPhone ?? string.Empty;
                    var codeSent = false;

                    if (!string.IsNullOrEmpty(emailForVerify))
                    {
                        if (info!.PreferredChannel.HasFlag(NotificationChannel.Email) || info.PreferredChannel == NotificationChannel.None)
                        {
                            var sendResult = await _accountService.SendEmailConfirmationCodeAsync(emailForVerify);
                            codeSent = sendResult.IsSuccess;
                            return RedirectToAction(nameof(VerifyEmail), new { email = emailForVerify, codeSent, showWarning = true });
                        }
                        if (info.PreferredChannel.HasFlag(NotificationChannel.Sms))
                        {
                            var sendResult = await _accountService.SendSmsConfirmationCodeAsync(emailForVerify);
                            codeSent = sendResult.IsSuccess;
                            return RedirectToAction(nameof(VerifySms), new { email = emailForVerify, maskedPhone, codeSent, showWarning = true });
                        }
                        if (info.PreferredChannel.HasFlag(NotificationChannel.WhatsApp))
                        {
                            var sendResult = await _accountService.SendWhatsAppConfirmationCodeAsync(emailForVerify);
                            codeSent = sendResult.IsSuccess;
                            return RedirectToAction(nameof(VerifyWhatsApp), new { email = emailForVerify, maskedPhone, codeSent, showWarning = true });
                        }
                    }
                    return RedirectToAction(nameof(VerifyEmail), new { email = emailForVerify, codeSent, showWarning = true });
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
            {
                if (result.Message == _localizer[Messages.Account_Email_Has_Not_Confirmed].Value)
                {
                    var infoResult = await _accountService.GetVerificationInfoAsync(model.UsernameOrEmail);
                    var info = infoResult.Data;
                    var emailForVerify = info?.Email ?? string.Empty;
                    var maskedPhone = info?.MaskedPhone ?? string.Empty;
                    var codeSent = false;
                    string? verifyUrl = null;

                    if (!string.IsNullOrEmpty(emailForVerify))
                    {
                        if (info!.PreferredChannel.HasFlag(NotificationChannel.Email) || info.PreferredChannel == NotificationChannel.None)
                        {
                            var sendResult = await _accountService.SendEmailConfirmationCodeAsync(emailForVerify);
                            codeSent = sendResult.IsSuccess;
                            verifyUrl = Url.Action(nameof(VerifyEmail), "Account", new { email = emailForVerify, codeSent, showWarning = true });
                        }
                        else if (info.PreferredChannel.HasFlag(NotificationChannel.Sms))
                        {
                            var sendResult = await _accountService.SendSmsConfirmationCodeAsync(emailForVerify);
                            codeSent = sendResult.IsSuccess;
                            verifyUrl = Url.Action(nameof(VerifySms), "Account", new { email = emailForVerify, maskedPhone, codeSent, showWarning = true });
                        }
                        else if (info.PreferredChannel.HasFlag(NotificationChannel.WhatsApp))
                        {
                            var sendResult = await _accountService.SendWhatsAppConfirmationCodeAsync(emailForVerify);
                            codeSent = sendResult.IsSuccess;
                            verifyUrl = Url.Action(nameof(VerifyWhatsApp), "Account", new { email = emailForVerify, maskedPhone, codeSent, showWarning = true });
                        }
                    }
                    verifyUrl ??= Url.Action(nameof(VerifyEmail), "Account", new { email = emailForVerify, showWarning = true });
                    return Json(new { success = false, requiresVerification = true, redirectUrl = verifyUrl });
                }
                return Json(new { success = false, message = result.Message ?? _localizer["Account_SignInError"].Value });
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
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            var redirectUrl = role == "Admin"
                ? Url.Action("Index", "Home", new { area = "Admin" })
                : Url.Action("Index", "Home", new { area = "Passenger" });

            return Json(new { success = true, redirectUrl });
        }

        [HttpGet]
        public IActionResult VerifyEmail(string? email, bool codeSent = false, bool showWarning = false)
        {
            return View(new VerifyEmailVM
            {
                Email       = email ?? string.Empty,
                CodeSent    = codeSent,
                ShowWarning = showWarning
            });
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
            return RedirectToAction("Index", "Home", new { area = "" });
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
        public IActionResult VerifySms(string? email, string? maskedPhone)
        {
            return View(new VerifyPhoneVM
            {
                Email       = email ?? string.Empty,
                MaskedPhone = maskedPhone ?? string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifySms(VerifyPhoneVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _accountService.VerifyEmailAsync(model.Email, model.Code);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(nameof(model.Code), result.Message ?? "");
                return View(model);
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendSmsConfirmation(string email, string maskedPhone)
        {
            var result = await _accountService.SendSmsConfirmationCodeAsync(email);
            if (result.IsSuccess)
                NotifySuccessLocalized(result.Message);
            else
                NotifyErrorLocalized(result.Message);
            return RedirectToAction(nameof(VerifySms), new { email, maskedPhone });
        }

        [HttpGet]
        public IActionResult VerifyWhatsApp(string? email, string? maskedPhone)
        {
            return View(new VerifyPhoneVM
            {
                Email       = email ?? string.Empty,
                MaskedPhone = maskedPhone ?? string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyWhatsApp(VerifyPhoneVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _accountService.VerifyEmailAsync(model.Email, model.Code);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(nameof(model.Code), result.Message ?? "");
                return View(model);
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendWhatsAppConfirmation(string email, string maskedPhone)
        {
            var result = await _accountService.SendWhatsAppConfirmationCodeAsync(email);
            if (result.IsSuccess)
                NotifySuccessLocalized(result.Message);
            else
                NotifyErrorLocalized(result.Message);
            return RedirectToAction(nameof(VerifyWhatsApp), new { email, maskedPhone });
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

            var maskedPhone = MaskPhone(model.PhoneNumber);
            if (!model.PreferredNotificationChannel.HasFlag(NotificationChannel.Email))
            {
                if (model.PreferredNotificationChannel.HasFlag(NotificationChannel.Sms))
                    return RedirectToAction(nameof(VerifySms), new { email = model.Email, maskedPhone, codeSent = true });
                if (model.PreferredNotificationChannel.HasFlag(NotificationChannel.WhatsApp))
                    return RedirectToAction(nameof(VerifyWhatsApp), new { email = model.Email, maskedPhone, codeSent = true });
            }
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
            NotifySuccess(_localizer[Messages.Account_SignOut_Successful]);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        private static string MaskPhone(string? phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 4)
                return phone ?? string.Empty;
            return new string('*', phone.Length - 4) + phone[^4..];
        }
    }
}
