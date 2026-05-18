namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAppUserService _appUserService;
        private readonly IStringLocalizer<MessageResources> _localizer;

        public AccountController(IAccountService accountService, IAppUserService appUserService, IStringLocalizer<MessageResources> localizer)
        {
            _accountService = accountService;
            _appUserService = appUserService;
            _localizer = localizer;
        }

        [HttpPost("sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto dto)
        {
            var result = await _accountService.SignUpAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("sign-in")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
        {
            var result = await _accountService.SignInAsync(dto);
            if (!result.IsSuccess)
            {
                if (result.Message == _localizer[Messages.Account_Email_Has_Not_Confirmed])
                    return StatusCode(StatusCodes.Status403Forbidden, result);
                return Unauthorized(result);
            }
            return Ok(result);
        }

        [HttpPost("sign-out")]
        [Authorize]
        public new async Task<IActionResult> SignOut()
        {
            var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _accountService.SignOutAsync(identityId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("send-verification-code")]
        [AllowAnonymous]
        public async Task<IActionResult> SendVerificationCode([FromQuery] string email, [FromQuery] VerificationCodePurpose purpose, [FromQuery] VerificationCodeChannel channel)
        {
            var result = await _accountService.SendVerificationCodeAsync(email, purpose, channel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("verify-code")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyEmailDto dto)
        {
            var result = await _accountService.VerifyCodeAsync(dto.Email, dto.Code, VerificationCodePurpose.EmailConfirmation);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _accountService.ResetPasswordAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDto dto)
        {
            var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userResult = await _appUserService.GetByIdentityIdAsync(identityId);
            if (!userResult.IsSuccess || userResult.Data == null)
                return NotFound(userResult);

            var current = userResult.Data;
            var updated = new AppUserDto(
                current.Id, dto.Name, dto.Surname, current.Email, dto.PhoneNumber,
                current.BirthDate, current.UserStatus, dto.NotificationPreference,
                current.Nationality, current.PassportNumber);

            var result = await _appUserService.UpdateAsync(current.Id, updated);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _accountService.ChangePasswordAsync(identityId, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("notification-preference")]
        [Authorize]
        public async Task<IActionResult> UpdateNotificationPreference([FromBody] NotificationPreferenceDto dto)
        {
            var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userResult = await _appUserService.GetByIdentityIdAsync(identityId);
            if (!userResult.IsSuccess || userResult.Data == null)
                return NotFound(userResult);

            var current = userResult.Data;
            var updated = new AppUserDto(
                current.Id, current.Name, current.Surname, current.Email, current.PhoneNumber,
                current.BirthDate, current.UserStatus, dto.NotificationPreference,
                current.Nationality, current.PassportNumber);

            var result = await _appUserService.UpdateAsync(current.Id, updated);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

