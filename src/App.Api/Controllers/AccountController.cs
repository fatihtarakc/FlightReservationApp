namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IStringLocalizer<MessageResources> _localizer;

        public AccountController(IAccountService accountService, IStringLocalizer<MessageResources> localizer)
        {
            _accountService = accountService;
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
    }
}

