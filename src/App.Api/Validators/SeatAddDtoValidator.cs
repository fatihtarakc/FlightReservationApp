namespace App.Api.Validators
{
    public class SeatAddDtoValidator : AbstractValidator<SeatAddDto>
    {
        public SeatAddDtoValidator()
        {
            RuleFor(x => x.Row)
                .InclusiveBetween(1, 200).WithMessage(Messages.Seat_Row_Invalid);

            RuleFor(x => x.Column)
                .IsInEnum().WithMessage(Messages.Seat_Please_Select_Column);

            RuleFor(x => x.SeatClass)
                .IsInEnum().WithMessage(Messages.Seat_Please_Select_Class);

            RuleFor(x => x.AircraftId)
                .NotEmpty().WithMessage(Messages.Seat_Please_Select_Aircraft);
        }
    }
}
