namespace App.Business.Concrete.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ITokenService _tokenService;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IAppUserRepository appUserRepository,
            IVerificationCodeService verificationCodeService,
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint,
            ITokenService tokenService,
            IStringLocalizer<MessageResources> localizer,
            ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appUserRepository = appUserRepository;
            _verificationCodeService = verificationCodeService;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
            _tokenService = tokenService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IResult> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return new ErrorResult(_localizer[Messages.Account_Email_Has_Already_Existed]);

            var identityUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = dto.Username,
                Email = dto.Email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!result.Succeeded)
                return new ErrorResult(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(identityUser, "AppUser");

            var appUser = new AppUser
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                BirthDate = dto.BirthDate,
                UserStatus = UserStatus.Active,
                PreferredNotificationChannel = dto.PreferredNotificationChannel,
                Nationality = dto.Nationality,
                IdentityId = identityUser.Id,
                CreatedBy = dto.Email
            };

            await _appUserRepository.AddAsync(appUser);
            await _unitOfWork.SaveChangesAsync();

            await _publishEndpoint.Publish(new UserRegisteredEvent
            {
                AppUserId = appUser.Id,
                Name = appUser.Name,
                Surname = appUser.Surname,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                PreferredChannel = appUser.PreferredNotificationChannel
            });

            var codeResult = await _verificationCodeService.GenerateAndSaveAsync(
                appUser.Id, VerificationCodePurpose.EmailConfirmation, VerificationCodeChannel.Email);

            if (codeResult.IsSuccess)
            {
                await _publishEndpoint.Publish(new VerificationCodeEvent
                {
                    Name = appUser.Name,
                    Email = appUser.Email,
                    PhoneNumber = appUser.PhoneNumber,
                    Code = codeResult.Data!,
                    Purpose = VerificationCodePurpose.EmailConfirmation,
                    Channel = VerificationCodeChannel.Email
                });
            }

            _logger.LogInformation("{Message} Email: {Email}", _localizer[Messages.AppUser_HasBeen_Added].Value, dto.Email);
            return new SuccessResult(_localizer[Messages.AppUser_HasBeen_Added]);
        }

        public async Task<IDataResult<TokenDto>> SignInAsync(SignInDto dto)
        {
            var identityUser = dto.UsernameOrEmail.Contains('@')
                ? await _userManager.FindByEmailAsync(dto.UsernameOrEmail)
                : await _userManager.FindByNameAsync(dto.UsernameOrEmail);

            if (identityUser == null)
                return new ErrorDataResult<TokenDto>(_localizer[Messages.Account_SignIn_Failed]);

            var signInResult = await _signInManager.CheckPasswordSignInAsync(identityUser, dto.Password, lockoutOnFailure: false);
            if (!signInResult.Succeeded)
                return new ErrorDataResult<TokenDto>(_localizer[Messages.Account_SignIn_Failed]);

            var roles = await _userManager.GetRolesAsync(identityUser);
            var tokenResult = _tokenService.GenerateToken(identityUser, roles);

            if (!tokenResult.IsSuccess)
                return new ErrorDataResult<TokenDto>(_localizer[Messages.Token_Could_Not_Generated]);

            _logger.LogInformation("{Message} UserId: {UserId}", _localizer[Messages.Account_SignIn_Successful].Value, identityUser.Id);
            return new SuccessDataResult<TokenDto>(tokenResult.Data!, _localizer[Messages.Account_SignIn_Successful]);
        }

        public async Task<IResult> SignOutAsync(string identityId)
        {
            var user = await _userManager.FindByIdAsync(identityId);
            if (user == null)
                return new ErrorResult(_localizer[Messages.Account_Was_Not_Found]);

            await _userManager.UpdateSecurityStampAsync(user);
            _logger.LogInformation("{Message} IdentityId: {Id}", _localizer[Messages.Account_SignOut_Successful].Value, identityId);
            return new SuccessResult(_localizer[Messages.Account_SignOut_Successful]);
        }

        public async Task<IResult> SendVerificationCodeAsync(string email, VerificationCodePurpose purpose, VerificationCodeChannel channel)
        {
            var appUser = await _appUserRepository.GetByEmailAsync(email, tracking: false);
            if (appUser == null)
                return new ErrorResult(_localizer[Messages.Account_Was_Not_Found]);

            var codeResult = await _verificationCodeService.GenerateAndSaveAsync(appUser.Id, purpose, channel);
            if (!codeResult.IsSuccess)
                return new ErrorResult(codeResult.Message);

            await _publishEndpoint.Publish(new VerificationCodeEvent
            {
                Name = appUser.Name,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                Code = codeResult.Data!,
                Purpose = purpose,
                Channel = channel
            });

            return new SuccessResult();
        }

        public async Task<IResult> VerifyCodeAsync(string email, string code, VerificationCodePurpose purpose)
        {
            var appUser = await _appUserRepository.GetByEmailAsync(email, tracking: false);
            if (appUser == null)
                return new ErrorResult(_localizer[Messages.Account_Was_Not_Found]);

            var result = await _verificationCodeService.ValidateAsync(appUser.Id, code, purpose);
            if (!result.IsSuccess)
                return result;

            if (purpose == VerificationCodePurpose.EmailConfirmation)
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser != null && !identityUser.EmailConfirmed)
                {
                    identityUser.EmailConfirmed = true;
                    await _userManager.UpdateAsync(identityUser);
                    _logger.LogInformation("{Message} Email: {Email}", _localizer[Messages.Account_Email_Was_Confirmed].Value, email);
                }
            }

            return new SuccessResult(_localizer[Messages.Account_ConfirmEmail_Successful]);
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var appUser = await _appUserRepository.GetByEmailAsync(dto.Email, tracking: false);
            if (appUser == null)
                return new ErrorResult(_localizer[Messages.Account_Was_Not_Found]);

            var validateResult = await _verificationCodeService.ValidateAsync(appUser.Id, dto.Code, VerificationCodePurpose.PasswordReset);
            if (!validateResult.IsSuccess)
                return validateResult;

            var identityUser = await _userManager.FindByEmailAsync(dto.Email);
            if (identityUser == null)
                return new ErrorResult(_localizer[Messages.Account_Was_Not_Found]);

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
            var result = await _userManager.ResetPasswordAsync(identityUser, resetToken, dto.NewPassword);

            if (!result.Succeeded)
                return new ErrorResult(_localizer[Messages.Account_ResetPassword_Failed]);

            _logger.LogInformation("{Message} Email: {Email}", _localizer[Messages.Account_ResetPassword_Successful].Value, dto.Email);
            return new SuccessResult(_localizer[Messages.Account_ResetPassword_Successful]);
        }
    }
}

