namespace App.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountWebService _accountService;

        public AccountController(IAccountWebService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(model.UsernameOrEmail) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Username/email and password are required.");
                return View(model);
            }

            var response = await _accountService.SignInAsync(model);

            if (response?.Success != true || response.Data == null)
            {
                ModelState.AddModelError(string.Empty, response?.Message ?? "Login failed.");
                return View(model);
            }

            var token = response.Data.AccessToken;
            await SignInWithTokenAsync(token, response.Data.Expiration);
            HttpContext.Session.SetString("jwt_token", token);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _accountService.RegisterAsync(model);

            if (response?.Success != true)
            {
                ModelState.AddModelError(string.Empty, response?.Message ?? "Registration failed.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration successful. Please check your email for a verification code.";
            return RedirectToAction(nameof(VerifyEmail), new { email = model.Email });
        }

        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            ViewBag.Email = email;
            return View(new VerifyEmailViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            var response = await _accountService.VerifyEmailAsync(model);

            if (response?.Success != true)
            {
                ModelState.AddModelError(string.Empty, response?.Message ?? "Verification failed.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Email verified successfully. You can now log in.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            await _accountService.SendVerificationCodeAsync(email, 1, 1);
            TempData["SuccessMessage"] = "If your email is registered, you will receive a reset code shortly.";
            return RedirectToAction(nameof(ResetPassword), new { email });
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View(new ResetPasswordViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var response = await _accountService.ResetPasswordAsync(model);

            if (response?.Success != true)
            {
                ModelState.AddModelError(string.Empty, response?.Message ?? "Password reset failed.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Password reset successfully. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("jwt_token");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();

        private async Task SignInWithTokenAsync(string token, DateTime expiration)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = new DateTimeOffset(expiration)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);
        }
    }
}
