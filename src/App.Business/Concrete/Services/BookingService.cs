namespace App.Business.Concrete.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository bookingRepository,
            IFlightRepository flightRepository,
            ISeatRepository seatRepository,
            IAppUserRepository appUserRepository,
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint,
            IStringLocalizer<MessageResources> localizer,
            ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _flightRepository = flightRepository;
            _seatRepository = seatRepository;
            _appUserRepository = appUserRepository;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<BookingDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var booking = await _bookingRepository.IncludeGetByIdAsync(id, tracking: false);
                if (booking == null)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.Booking_Was_Not_Found]);

                return new SuccessDataResult<BookingDto>(booking.Adapt<BookingDto>(), _localizer[Messages.Booking_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<BookingDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<BookingDto>> GetByPnrAsync(string pnr)
        {
            try
            {
                var booking = await _bookingRepository.GetByPnrAsync(pnr, tracking: false);
                if (booking == null)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.Booking_Was_Not_Found]);

                return new SuccessDataResult<BookingDto>(booking.Adapt<BookingDto>(), _localizer[Messages.Booking_Was_Found]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<BookingDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<BookingListDto>>> GetByUserIdAsync(Guid userId)
        {
            try
            {
                var bookings = await _bookingRepository.GetByUserIdAsync(userId, tracking: false);
                return new SuccessDataResult<IEnumerable<BookingListDto>>(bookings.Select(b => b.Adapt<BookingListDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<BookingListDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<BookingDto>>> GetAllAsync()
        {
            try
            {
                var bookings = await _bookingRepository.GetAllWithDetailsAsync(tracking: false);
                return new SuccessDataResult<IEnumerable<BookingDto>>(bookings.Select(b => b.Adapt<BookingDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<BookingDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<IEnumerable<BookingListDto>>> GetByFlightIdAsync(Guid flightId)
        {
            try
            {
                var bookings = await _bookingRepository.GetActiveBookingsByFlightIdAsync(flightId, tracking: false);
                return new SuccessDataResult<IEnumerable<BookingListDto>>(bookings.Select(b => b.Adapt<BookingListDto>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<IEnumerable<BookingListDto>>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IDataResult<BookingDto>> AddAsync(Guid userId, BookingAddDto dto)
        {
            try
            {
                var user = await _appUserRepository.GetByIdAsync(userId, tracking: false);
                if (user == null)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.AppUser_Was_Not_Found]);

                var flight = await _flightRepository.IncludeGetByIdAsync(dto.FlightId, tracking: false);
                if (flight == null)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.Flight_Was_Not_Found]);

                if (flight.FlightStatus != FlightStatus.Scheduled)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.Flight_No_Available_Seats]);

                var alreadyBooked = await _bookingRepository.AnyAsync(b =>
                    b.AppUserId == userId && b.FlightId == dto.FlightId &&
                    b.BookingStatus != BookingStatus.Cancelled);
                if (alreadyBooked)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.Booking_Already_Exists_For_This_Flight]);

                var seat = await _seatRepository.GetByIdAsync(dto.SeatId, tracking: false);
                if (seat == null)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.Seat_Was_Not_Found]);

                var seatTaken = await _bookingRepository.AnyAsync(b =>
                    b.SeatId == dto.SeatId && b.FlightId == dto.FlightId &&
                    b.BookingStatus != BookingStatus.Cancelled);
                if (seatTaken)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.Flight_Seat_Is_Already_Booked]);

                var price = seat.SeatClass switch
                {
                    SeatClass.Economy        => flight.BaseEconomyPrice,
                    SeatClass.PremiumEconomy => flight.BasePremiumEconomyPrice,
                    SeatClass.Business       => flight.BaseBusinessPrice,
                    SeatClass.First          => flight.BaseFirstClassPrice,
                    _                        => flight.BaseEconomyPrice
                };

                IDataResult<BookingDto> result = new ErrorDataResult<BookingDto>(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var booking = new Booking
                        {
                            PnrNumber     = GeneratePnr(),
                            TotalPrice    = price,
                            Currency      = flight.Currency,
                            BookingStatus = BookingStatus.Confirmed,
                            AppUserId     = userId,
                            FlightId      = dto.FlightId,
                            SeatId        = dto.SeatId,
                            CreatedBy     = user.Email!
                        };

                        await _bookingRepository.AddAsync(booking);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        var seatNumber = $"{seat.Row}{seat.Column}";
                        await _publishEndpoint.Publish(new BookingConfirmedEvent
                        {
                            BookingId         = booking.Id,
                            PnrNumber         = booking.PnrNumber,
                            PassengerName     = $"{user.Name} {user.Surname}",
                            Email             = user.Email!,
                            PhoneNumber       = user.PhoneNumber!,
                            FlightNumber      = flight.Number,
                            DepartureDateTime = flight.DepartureDateTime,
                            DepartureCity     = flight.Schedule?.Route?.DepartureAirport?.City ?? string.Empty,
                            ArrivalCity       = flight.Schedule?.Route?.ArrivalAirport?.City ?? string.Empty,
                            SeatNumber        = seatNumber,
                            SeatClass         = seat.SeatClass,
                            TotalPrice        = price,
                            PreferredChannel  = user.PreferredNotificationChannel
                        });

                        var saved = await _bookingRepository.IncludeGetByIdAsync(booking.Id, tracking: false);
                        _logger.LogInformation("{Message} PNR: {Pnr}", _localizer[Messages.Booking_HasBeen_Added].Value, booking.PnrNumber);
                        result = new SuccessDataResult<BookingDto>(saved.Adapt<BookingDto>(), _localizer[Messages.Booking_HasBeen_Added]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} UserId: {UserId}", _localizer[Messages.UnexpectedError].Value, userId);
                        result = new ErrorDataResult<BookingDto>(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<BookingDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        public async Task<IResult> CancelAsync(Guid id, string? reason)
        {
            try
            {
                var booking = await _bookingRepository.IncludeGetByIdAsync(id);
                if (booking == null)
                    return new ErrorResult(_localizer[Messages.Booking_Was_Not_Found]);

                if (booking.Flight!.DepartureDateTime <= DateTime.UtcNow)
                    return new ErrorResult(_localizer[Messages.Booking_Cannot_Cancel_Departed]);

                if (booking.BookingStatus == BookingStatus.Cancelled)
                    return new ErrorResult(_localizer[Messages.Booking_Could_Not_Be_Cancelled]);

                IResult result = new ErrorResult(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        booking.BookingStatus      = BookingStatus.Cancelled;
                        booking.CancellationReason = reason;

                        await _bookingRepository.UpdateAsync(booking);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        await _publishEndpoint.Publish(new BookingCancelledEvent
                        {
                            BookingId          = booking.Id,
                            PnrNumber          = booking.PnrNumber,
                            PassengerName      = $"{booking.AppUser!.Name} {booking.AppUser!.Surname}",
                            Email              = booking.AppUser.Email!,
                            PhoneNumber        = booking.AppUser.PhoneNumber!,
                            FlightNumber       = booking.Flight.Number,
                            DepartureDateTime  = booking.Flight.DepartureDateTime,
                            DepartureCity      = booking.Flight?.Schedule?.Route?.DepartureAirport?.City ?? string.Empty,
                            ArrivalCity        = booking.Flight?.Schedule?.Route?.ArrivalAirport?.City ?? string.Empty,
                            CancellationReason = reason,
                            PreferredChannel   = booking.AppUser.PreferredNotificationChannel
                        });

                        _logger.LogInformation("{Message} BookingId: {Id}", _localizer[Messages.Booking_Was_Cancelled].Value, id);
                        result = new SuccessResult(_localizer[Messages.Booking_Was_Cancelled]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} BookingId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
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

        public async Task<IDataResult<BookingDto>> CheckInAsync(Guid id)
        {
            try
            {
                var booking = await _bookingRepository.IncludeGetByIdAsync(id);
                if (booking == null)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.Booking_Was_Not_Found]);

                if (booking.BookingStatus == BookingStatus.CheckedIn)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.Booking_Already_CheckedIn]);

                if (booking.BookingStatus != BookingStatus.Confirmed)
                    return new ErrorDataResult<BookingDto>(_localizer[Messages.Booking_Cannot_CheckIn_NotConfirmed]);

                IDataResult<BookingDto> result = new ErrorDataResult<BookingDto>(_localizer[Messages.UnexpectedError]);
                var strategy = await _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        booking.BookingStatus = BookingStatus.CheckedIn;
                        booking.CheckInTime   = DateTime.UtcNow;

                        await _bookingRepository.UpdateAsync(booking);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        var updated = await _bookingRepository.IncludeGetByIdAsync(id, tracking: false);
                        _logger.LogInformation("{Message} BookingId: {Id}", _localizer[Messages.Booking_CheckedIn_Successfully].Value, id);
                        result = new SuccessDataResult<BookingDto>(updated!.Adapt<BookingDto>(), _localizer[Messages.Booking_CheckedIn_Successfully]);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "{Message} BookingId: {Id}", _localizer[Messages.UnexpectedError].Value, id);
                        result = new ErrorDataResult<BookingDto>(_localizer[Messages.UnexpectedError]);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[Messages.UnexpectedError]);
                return new ErrorDataResult<BookingDto>(_localizer[Messages.UnexpectedError]);
            }
        }

        private static string GeneratePnr()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Range(0, 6).Select(_ => chars[Random.Shared.Next(chars.Length)]).ToArray());
        }
    }
}
