namespace App.Business.Concrete.Services
{
    public class VerificationCodeService : IVerificationCodeService
    {
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<VerificationCodeService> _logger;

        public VerificationCodeService(
            IVerificationCodeRepository verificationCodeRepository,
            IAppUserRepository appUserRepository,
            IUnitOfWork unitOfWork,
            IStringLocalizer<MessageResources> localizer,
            ILogger<VerificationCodeService> logger)
        {
            _verificationCodeRepository = verificationCodeRepository;
            _appUserRepository = appUserRepository;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<string>> GenerateAndSaveAsync(Guid userId, VerificationCodePurpose purpose, VerificationCodeChannel channel)
        {
            var existingCode = await _verificationCodeRepository.GetActiveCodeAsync(userId, purpose);
            if (existingCode != null)
            {
                existingCode.Status = VerificationCodeStatus.Expired;
                await _verificationCodeRepository.UpdateAsync(existingCode);
            }

            var code = GenerateCode();
            var verificationCode = new VerificationCode
            {
                Code = code,
                Channel = channel,
                Purpose = purpose,
                Status = VerificationCodeStatus.Active,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                AttemptCount = 0,
                AppUserId = userId
            };

            await _verificationCodeRepository.AddAsync(verificationCode);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessDataResult<string>(code);
        }

        public async Task<IResult> ValidateAsync(Guid userId, string code, VerificationCodePurpose purpose)
        {
            var verificationCode = await _verificationCodeRepository.GetActiveCodeAsync(userId, purpose);

            if (verificationCode == null)
                return new ErrorResult(_localizer[Messages.Account_Please_Take_New_VerificationCode]);

            if (verificationCode.ExpiresAt < DateTime.UtcNow)
            {
                verificationCode.Status = VerificationCodeStatus.Expired;
                await _verificationCodeRepository.UpdateAsync(verificationCode);
                await _unitOfWork.SaveChangesAsync();
                return new ErrorResult(_localizer[Messages.Account_VerificationCode_Is_Expired]);
            }

            if (verificationCode.Code != code)
            {
                verificationCode.AttemptCount++;
                await _verificationCodeRepository.UpdateAsync(verificationCode);
                await _unitOfWork.SaveChangesAsync();
                return new ErrorResult(_localizer[Messages.Account_VerificationCode_Is_Invalid]);
            }

            verificationCode.Status = VerificationCodeStatus.Used;
            await _verificationCodeRepository.UpdateAsync(verificationCode);
            await _unitOfWork.SaveChangesAsync();
            return new SuccessResult();
        }

        private static string GenerateCode() =>
            string.Concat(Enumerable.Range(0, 6).Select(_ => Random.Shared.Next(0, 10).ToString()));
    }
}
