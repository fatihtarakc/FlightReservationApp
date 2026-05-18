namespace App.Business.Concrete.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<AppUserService> _logger;

        public AppUserService(
            IAppUserRepository appUserRepository,
            IUnitOfWork unitOfWork,
            IStringLocalizer<MessageResources> localizer,
            ILogger<AppUserService> logger)
        {
            _appUserRepository = appUserRepository;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<AppUserDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _appUserRepository.GetByIdAsync(id, tracking: false);
                if (user == null)
                    return new ErrorDataResult<AppUserDto>(_localizer[Messages.AppUser_Was_Not_Found]);

                return new SuccessDataResult<AppUserDto>(user.Adapt<AppUserDto>(), _localizer[Messages.AppUser_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<AppUserDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<AppUserDto>> GetByIdentityIdAsync(string identityId)
        {
            try
            {
                var user = await _appUserRepository.GetByIdentityIdAsync(identityId, tracking: false);
                if (user == null)
                    return new ErrorDataResult<AppUserDto>(_localizer[Messages.AppUser_Was_Not_Found]);

                return new SuccessDataResult<AppUserDto>(user.Adapt<AppUserDto>(), _localizer[Messages.AppUser_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<AppUserDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<AppUserListDto>>> GetAllAsync()
        {
            try
            {
                var users = await _appUserRepository.GetAllAsync(tracking: false);
                return new SuccessDataResult<IEnumerable<AppUserListDto>>(users.Select(u => u.Adapt<AppUserListDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<AppUserListDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> UpdateAsync(Guid id, AppUserDto dto)
        {
            try
            {
                var user = await _appUserRepository.GetByIdAsync(id);
                if (user == null)
                    return new ErrorResult(_localizer[Messages.AppUser_Was_Not_Found]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        user.Name                        = dto.Name;
                        user.Surname                     = dto.Surname;
                        user.PhoneNumber                 = dto.PhoneNumber;
                        user.BirthDate                   = dto.BirthDate;
                        user.PreferredNotificationChannel = dto.PreferredNotificationChannel;
                        user.Nationality                 = dto.Nationality;
                        user.PassportNumber              = dto.PassportNumber;

                        await _appUserRepository.UpdateAsync(user);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("{Message} UserId: {Id}", _localizer[Messages.AppUser_Was_Updated].Value, id);
                        result = new SuccessResult(_localizer[Messages.AppUser_Was_Updated]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} UserId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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

        public async Task<IResult> SetStatusAsync(Guid id, bool isActive)
        {
            try
            {
                var user = await _appUserRepository.GetByIdAsync(id);
                if (user == null)
                    return new ErrorResult(_localizer[Messages.AppUser_Was_Not_Found]);

                user.UserStatus = isActive ? UserStatus.Active : UserStatus.Suspended;
                await _appUserRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var msg = isActive
                    ? _localizer[Messages.AppUser_Was_Activated]
                    : _localizer[Messages.AppUser_Was_Deactivated];
                _logger.LogInformation("{Message} UserId: {Id}", msg.Value, id);
                return new SuccessResult(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorResult(_localizer[Messages.UnexpectedError]);
            }
        }
    }
}
