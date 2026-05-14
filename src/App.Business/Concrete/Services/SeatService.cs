using Mapster;
namespace App.Business.Concrete.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<SeatService> _logger;

        public SeatService(
            ISeatRepository seatRepository,
            IUnitOfWork unitOfWork,
            IStringLocalizer<MessageResources> localizer,
            ILogger<SeatService> logger)
        {
            _seatRepository = seatRepository;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<SeatDto>> GetByIdAsync(Guid id)
        {
            var seat = await _seatRepository.GetByIdAsync(id, tracking: false);
            if (seat == null)
                return new ErrorDataResult<SeatDto>(_localizer[Messages.Seat_Was_Not_Found]);

            return new SuccessDataResult<SeatDto>(seat.Adapt<SeatDto>(), _localizer[Messages.Seat_Was_Found]);
        }

        public async Task<IDataResult<IEnumerable<SeatDto>>> GetByAircraftIdAsync(Guid aircraftId)
        {
            var seats = await _seatRepository.GetByAircraftIdAsync(aircraftId, tracking: false);
            return new SuccessDataResult<IEnumerable<SeatDto>>(seats.Select(s => s.Adapt<SeatDto>()));
        }

        public async Task<IDataResult<IEnumerable<SeatDto>>> GetAvailableByFlightIdAsync(Guid flightId)
        {
            var seats = await _seatRepository.GetAvailableSeatsByFlightIdAsync(flightId, tracking: false);
            return new SuccessDataResult<IEnumerable<SeatDto>>(seats.Select(s => s.Adapt<SeatDto>() with { IsAvailable = true }));
        }

        public async Task<IResult> AddAsync(SeatAddDto dto)
        {
            var seat = new Seat
            {
                Row = dto.Row,
                Column = dto.Column,
                SeatClass = dto.SeatClass,
                IsWindowSeat = dto.IsWindowSeat,
                IsAisleSeat = dto.IsAisleSeat,
                HasExtraLegRoom = dto.HasExtraLegRoom,
                AircraftId = dto.AircraftId
            };

            await _seatRepository.AddAsync(seat);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{Message} Row: {Row} Column: {Col}", _localizer[Messages.Seat_HasBeen_Added].Value, dto.Row, dto.Column);
            return new SuccessResult(_localizer[Messages.Seat_HasBeen_Added]);
        }

        public async Task<IResult> AddRangeAsync(IEnumerable<SeatAddDto> dtos)
        {
            var seats = dtos.Select(dto => new Seat
            {
                Row = dto.Row,
                Column = dto.Column,
                SeatClass = dto.SeatClass,
                IsWindowSeat = dto.IsWindowSeat,
                IsAisleSeat = dto.IsAisleSeat,
                HasExtraLegRoom = dto.HasExtraLegRoom,
                AircraftId = dto.AircraftId
            });

            await _seatRepository.AddRangeAsync(seats);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult(_localizer[Messages.Seat_HasBeen_Added]);
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);
            if (seat == null)
                return new ErrorResult(_localizer[Messages.Seat_Was_Not_Found]);

            await _seatRepository.DeleteAsync(seat);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult(_localizer[Messages.Seat_Was_Deleted]);
        }
    }
}



