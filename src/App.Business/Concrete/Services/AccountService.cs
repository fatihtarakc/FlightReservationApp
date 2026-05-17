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

        public async Task<IResult> SignUpAsync(SignUpDto dto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                    return new ErrorResult(_localizer[Messages.Account_Email_Has_Already_Existed]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var identityUser = new IdentityUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = dto.Username,
                            Email = dto.Email,
                            EmailConfirmed = false
                        };

                        var createResult = await _userManager.CreateAsync(identityUser, dto.Password);
                        if (!createResult.Succeeded)
                        {
                            result = new ErrorResult(string.Join(", ", createResult.Errors.Select(e => e.Description)));
                            await transaction.RollbackAsync();
                            return;
                        }

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
                        await transaction.CommitAsync();

                        await _publishEndpoint.Publish(new UserSignedUpEvent
                        {
                            AppUserId = appUser.Id,
                            Name = appUser.Name,
                            Surname = appUser.Surname,
                            Email = appUser.Email,
                            PhoneNumber = appUser.PhoneNumber,
                            PreferredChannel = appUser.PreferredNotificationChannel,
                            Language = CultureInfo.CurrentUICulture.Name
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
                                Channel = VerificationCodeChannel.Email,
                                Language = CultureInfo.CurrentUICulture.Name
                            });
                        }

                        _logger.LogInformation("{Message} Email: {Email}", _localizer[Messages.AppUser_HasBeen_Added].Value, dto.Email);
                        result = new SuccessResult(_localizer[Messages.AppUser_HasBeen_Added]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} Email: {Email}", _localizer[Messages.UnexpectedError].Value, dto.Email);
                        result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<TokenDto>> SignInAsync(SignInDto dto)
        {
            try
            {
                var identityUser = dto.UsernameOrEmail.Contains('@')
                    ? await _userManager.FindByEmailAsync(dto.UsernameOrEmail)
                    : await _userManager.FindByNameAsync(dto.UsernameOrEmail);

                if (identityUser == null)
                    return new ErrorDataResult<TokenDto>(_localizer[Messages.Account_SignIn_Failed]);

                var signInResult = await _signInManager.CheckPasswordSignInAsync(identityUser, dto.Password, lockoutOnFailure: false);
                if (!signInResult.Succeeded)
                    return new ErrorDataResult<TokenDto>(_localizer[Messages.Account_SignIn_Failed]);

                if (!identityUser.EmailConfirmed)
                    return new ErrorDataResult<TokenDto>(_localizer[Messages.Account_Email_Has_Not_Confirmed]);

                var roles = await _userManager.GetRolesAsync(identityUser);
                var tokenResult = _tokenService.GenerateToken(identityUser, roles);

                if (!tokenResult.IsSuccess)
                    return new ErrorDataResult<TokenDto>(_localizer[Messages.Token_Could_Not_Generated]);

                _logger.LogInformation("{Message} UserId: {UserId}", _localizer[Messages.Account_SignIn_Successful].Value, identityUser.Id);
                return new SuccessDataResult<TokenDto>(tokenResult.Data!, _localizer[Messages.Account_SignIn_Successful]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<TokenDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> SignOutAsync(string identityId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(identityId);
                if (user == null)
                    return new ErrorResult(_localizer[Messages.Account_Was_Not_Found]);

                await _userManager.UpdateSecurityStampAsync(user);
                _logger.LogInformation("{Message} IdentityId: {Id}", _localizer[Messages.Account_SignOut_Successful].Value, identityId);
                return new SuccessResult(_localizer[Messages.Account_SignOut_Successful]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> SendVerificationCodeAsync(string email, VerificationCodePurpose purpose, VerificationCodeChannel channel)
        {
            try
            {
                var appUser = await _appUserRepository.GetByEmailAsync(email, tracking: false);
                if (appUser == null)
                {
                    var identityUser = await _userManager.FindByEmailAsync(email);
                    if (identityUser != null && await _userManager.IsInRoleAsync(identityUser, "Admin"))
                        return new ErrorResult(_localizer[Messages.Account_Privileged_Cannot_Use_This_Feature]);

                    return new ErrorResult(_localizer[Messages.Account_Was_Not_Found]);
                }

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
                    Channel = channel,
                    Language = CultureInfo.CurrentUICulture.Name
                });

                return new SuccessResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> VerifyCodeAsync(string email, string code, VerificationCodePurpose purpose)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
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

                await _publishEndpoint.Publish(new PasswordChangedEvent
                {
                    Name = appUser.Name,
                    Email = appUser.Email,
                    PhoneNumber = appUser.PhoneNumber,
                    ChangedAt = DateTime.UtcNow,
                    PreferredChannel = appUser.PreferredNotificationChannel,
                    Language = CultureInfo.CurrentUICulture.Name
                });

                _logger.LogInformation("{Message} Email: {Email}", _localizer[Messages.Account_ResetPassword_Successful].Value, dto.Email);
                return new SuccessResult(_localizer[Messages.Account_ResetPassword_Successful]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }
    }
}
