namespace App.Api.Validators
{
    public class FlightAddDtoValidator : AbstractValidator<FlightAddDto>
    {
        public FlightAddDtoValidator()
        {
            RuleFor(x => x.Number)
                .NotEmpty().WithMessage(Messages.Flight_Please_Enter_Number)
                .MaximumLength(6).WithMessage(Messages.Flight_Number_Cannot_Be_GreaterThan_6Characters)
                .Matches(@"^[A-Z0-9]{3,6}$").WithMessage(Messages.Flight_Number_Format_Invalid);

            RuleFor(x => x.DepartureDateTime)
                .NotEmpty().WithMessage(Messages.Flight_DepartureDateTime_Required)
                .GreaterThan(DateTime.UtcNow).WithMessage(Messages.Flight_DepartureDateTime_Must_Be_Future);

            RuleFor(x => x.ArrivalDateTime)
                .NotEmpty().WithMessage(Messages.Flight_ArrivalDateTime_Required)
                .GreaterThan(x => x.DepartureDateTime).WithMessage(Messages.Flight_ArrivalDateTime_Must_Be_After_Departure);

            RuleFor(x => x.BaseEconomyPrice)
                .GreaterThan(0).WithMessage(Messages.Flight_EconomyPrice_Must_Be_Positive);

            RuleFor(x => x.BasePremiumEconomyPrice)
                .GreaterThanOrEqualTo(0).WithMessage(Messages.Flight_PremiumEconomyPrice_Must_Be_Positive);

            RuleFor(x => x.BaseBusinessPrice)
                .GreaterThanOrEqualTo(0).WithMessage(Messages.Flight_BusinessPrice_Must_Be_Positive);

            RuleFor(x => x.BaseFirstClassPrice)
                .GreaterThanOrEqualTo(0).WithMessage(Messages.Flight_FirstClassPrice_Must_Be_Positive);

            RuleFor(x => x.Currency)
                .IsInEnum().WithMessage(Messages.Flight_Please_Select_Currency);

            RuleFor(x => x.Gate)
                .MaximumLength(10).WithMessage(Messages.Flight_Gate_Cannot_Be_GreaterThan_10Characters)
                .When(x => x.Gate != null);

            RuleFor(x => x.Terminal)
                .MaximumLength(10).WithMessage(Messages.Flight_Terminal_Cannot_Be_GreaterThan_10Characters)
                .When(x => x.Terminal != null);

            RuleFor(x => x.AircraftId)
                .NotEmpty().WithMessage(Messages.Flight_Please_Select_Aircraft);

            RuleFor(x => x.AirlineId)
                .NotEmpty().WithMessage(Messages.Flight_Please_Select_Airline);

            RuleFor(x => x.ScheduleId)
                .NotEmpty().WithMessage(Messages.Flight_Please_Select_Schedule);
        }
    }
}
