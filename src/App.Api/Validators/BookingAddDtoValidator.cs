using FluentValidation;

namespace App.Api.Validators
{
    public class BookingAddDtoValidator : AbstractValidator<BookingAddDto>
    {
        public BookingAddDtoValidator()
        {
            RuleFor(x => x.FlightId)
                .NotEmpty().WithMessage("Flight selection is required.");

            RuleFor(x => x.SeatId)
                .NotEmpty().WithMessage("Seat selection is required.");
        }
    }
}
