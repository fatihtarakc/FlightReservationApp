namespace App.Business.Concrete.Services
{
    public class AircraftService : IAircraftService
    {
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<AircraftService> _logger;

        public AircraftService(
            IAircraftRepository aircraftRepository,
            IUnitOfWork unitOfWork,
            IStringLocalizer<MessageResources> localizer,
            ILogger<AircraftService> logger)
        {
            _aircraftRepository = aircraftRepository;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<AircraftDto>> GetByIdAsync(Guid id)
        {
            var aircraft = await _aircraftRepository.IncludeGetByIdAsync(id, tracking: false);
            if (aircraft == null)
                return new ErrorDataResult<AircraftDto>(_localizer[Messages.Aircraft_Was_Not_Found]);

            return new SuccessDataResult<AircraftDto>(aircraft.Adapt<AircraftDto>(), _localizer[Messages.Aircraft_Was_Found]);
        }

        public async Task<IDataResult<IEnumerable<AircraftListDto>>> GetAllAsync()
        {
            var aircrafts = await _aircraftRepository.GetAllAsync(tracking: false);
            return new SuccessDataResult<IEnumerable<AircraftListDto>>(aircrafts.Select(a => a.Adapt<AircraftListDto>()));
        }

        public async Task<IDataResult<AircraftDto>> AddAsync(AircraftAddDto dto)
        {
            var exists = await _aircraftRepository.AnyAsync(a => a.TailNumber == dto.TailNumber);
            if (exists)
                return new ErrorDataResult<AircraftDto>(_localizer[Messages.Aircraft_TailNumber_Already_Exists]);

            var aircraft = new Aircraft
            {
                TailNumber = dto.TailNumber,
                ManufactureYear = dto.ManufactureYear,
                AircraftStatus = AircraftStatus.Active,
                AirlineId = dto.AirlineId,
                ModelId = dto.ModelId
            };

            await _aircraftRepository.AddAsync(aircraft);
            await _unitOfWork.SaveChangesAsync();

            var saved = await _aircraftRepository.IncludeGetByIdAsync(aircraft.Id, tracking: false);
            _logger.LogInformation("{Message} TailNumber: {Tail}", _localizer[Messages.Aircraft_HasBeen_Added].Value, dto.TailNumber);
            return new SuccessDataResult<AircraftDto>(saved.Adapt<AircraftDto>(), _localizer[Messages.Aircraft_HasBeen_Added]);
        }

        public async Task<IResult> UpdateAsync(Guid id, AircraftUpdateDto dto)
        {
            var aircraft = await _aircraftRepository.GetByIdAsync(id);
            if (aircraft == null)
                return new ErrorResult(_localizer[Messages.Aircraft_Was_Not_Found]);

            aircraft.TailNumber = dto.TailNumber;
            aircraft.ManufactureYear = dto.ManufactureYear;
            aircraft.AircraftStatus = dto.AircraftStatus;
            aircraft.AirlineId = dto.AirlineId;
            aircraft.ModelId = dto.ModelId;

            await _aircraftRepository.UpdateAsync(aircraft);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{Message} AircraftId: {Id}", _localizer[Messages.Aircraft_Was_Updated].Value, id);
            return new SuccessResult(_localizer[Messages.Aircraft_Was_Updated]);
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var aircraft = await _aircraftRepository.GetByIdAsync(id);
            if (aircraft == null)
                return new ErrorResult(_localizer[Messages.Aircraft_Was_Not_Found]);

            await _aircraftRepository.DeleteAsync(aircraft);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{Message} AircraftId: {Id}", _localizer[Messages.Aircraft_Was_Deleted].Value, id);
            return new SuccessResult(_localizer[Messages.Aircraft_Was_Deleted]);
        }
    }
}


