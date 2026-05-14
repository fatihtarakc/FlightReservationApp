namespace App.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiService _apiService;

        public AccountController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(SignInDto model, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(model.UsernameOrEmail) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Username/email and password are required.");
                return View(model);
            }

            var response = await _apiService.PostAsync<TokenDto>("account/sign-in", model);

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

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiService.PostAsync<object>("account/register", model);

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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiService.PostAsync<object>("account/verify-email", model);

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
            var response = await _apiService.PostAsync<object>("account/send-verification-code", new { email });

            TempData["SuccessMessage"] = "If your email is registered, you will receive a reset code shortly.";
            return RedirectToAction(nameof(ResetPassword), new { email });
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiService.PostAsync<object>("account/reset-password", model);

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
