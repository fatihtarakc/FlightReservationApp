namespace App.Api.Validators
{
    public class FlightSearchDtoValidator : AbstractValidator<FlightSearchDto>
    {
        public FlightSearchDtoValidator()
        {
            RuleFor(x => x.DepartureIata)
                .NotEmpty().WithMessage(Messages.FlightSearch_DepartureIata_Required)
                .Length(3).WithMessage(Messages.FlightSearch_DepartureIata_Format_Invalid)
                .Matches(@"^[A-Z]{3}$").WithMessage(Messages.FlightSearch_DepartureIata_Format_Invalid);

            RuleFor(x => x.ArrivalIata)
                .NotEmpty().WithMessage(Messages.FlightSearch_ArrivalIata_Required)
                .Length(3).WithMessage(Messages.FlightSearch_ArrivalIata_Format_Invalid)
                .Matches(@"^[A-Z]{3}$").WithMessage(Messages.FlightSearch_ArrivalIata_Format_Invalid);

            RuleFor(x => x.DepartureDate)
                .NotEmpty().WithMessage(Messages.FlightSearch_DepartureDate_Required);

            RuleFor(x => x.Passengers)
                .InclusiveBetween(1, 9).WithMessage(Messages.FlightSearch_Passengers_Invalid);

            RuleFor(x => x.SeatClass)
                .IsInEnum().WithMessage(Messages.FlightSearch_SeatClass_Invalid);
        }
    }
}
