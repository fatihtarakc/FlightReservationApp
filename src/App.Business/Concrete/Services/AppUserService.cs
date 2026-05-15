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
            var user = await _appUserRepository.GetByIdAsync(id, tracking: false);
            if (user == null)
                return new ErrorDataResult<AppUserDto>(_localizer[Messages.AppUser_Was_Not_Found]);

            return new SuccessDataResult<AppUserDto>(user.Adapt<AppUserDto>(), _localizer[Messages.AppUser_Was_Found]);
        }

        public async Task<IDataResult<AppUserDto>> GetByIdentityIdAsync(string identityId)
        {
            var user = await _appUserRepository.GetByIdentityIdAsync(identityId, tracking: false);
            if (user == null)
                return new ErrorDataResult<AppUserDto>(_localizer[Messages.AppUser_Was_Not_Found]);

            return new SuccessDataResult<AppUserDto>(user.Adapt<AppUserDto>(), _localizer[Messages.AppUser_Was_Found]);
        }

        public async Task<IDataResult<IEnumerable<AppUserListDto>>> GetAllAsync()
        {
            var users = await _appUserRepository.GetAllAsync(tracking: false);
            return new SuccessDataResult<IEnumerable<AppUserListDto>>(users.Select(u => u.Adapt<AppUserListDto>()));
        }

        public async Task<IResult> UpdateAsync(Guid id, AppUserDto dto)
        {
            var user = await _appUserRepository.GetByIdAsync(id);
            if (user == null)
                return new ErrorResult(_localizer[Messages.AppUser_Was_Not_Found]);

            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.PhoneNumber = dto.PhoneNumber;
            user.BirthDate = dto.BirthDate;
            user.PreferredNotificationChannel = dto.PreferredNotificationChannel;
            user.Nationality = dto.Nationality;
            user.PassportNumber = dto.PassportNumber;

            await _appUserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{Message} UserId: {Id}", _localizer[Messages.AppUser_Was_Updated].Value, id);
            return new SuccessResult(_localizer[Messages.AppUser_Was_Updated]);
        }
    }
}


