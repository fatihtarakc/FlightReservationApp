namespace App.Api.Validators
{
    public class BookingAddDtoValidator : AbstractValidator<BookingAddDto>
    {
        public BookingAddDtoValidator()
        {
            RuleFor(x => x.FlightId)
                .NotEmpty().WithMessage(Messages.Booking_Please_Select_Flight);

            RuleFor(x => x.SeatId)
                .NotEmpty().WithMessage(Messages.Booking_Please_Select_Seat);
        }
    }
}
