namespace App.Api.Validators
{
    public class FlightAddDtoValidator : AbstractValidator<FlightAddDto>
    {
        public FlightAddDtoValidator(IStringLocalizer<MessageResources> localizer)
        {
            RuleFor(x => x.Number)
                .NotEmpty().WithMessage(x => localizer[Messages.Flight_Please_Enter_Number])
                .MaximumLength(6).WithMessage(x => localizer[Messages.Flight_Number_Cannot_Be_GreaterThan_6Characters])
                .Matches(@"^[A-Z0-9]{3,6}$").WithMessage(x => localizer[Messages.Flight_Number_Format_Invalid]);

            RuleFor(x => x.DepartureDateTime)
                .NotEmpty().WithMessage(x => localizer[Messages.Flight_DepartureDateTime_Required])
                .GreaterThan(DateTime.UtcNow).WithMessage(x => localizer[Messages.Flight_DepartureDateTime_Must_Be_Future]);

            RuleFor(x => x.ArrivalDateTime)
                .NotEmpty().WithMessage(x => localizer[Messages.Flight_ArrivalDateTime_Required])
                .GreaterThan(x => x.DepartureDateTime).WithMessage(x => localizer[Messages.Flight_ArrivalDateTime_Must_Be_After_Departure]);

            RuleFor(x => x.BaseEconomyPrice)
                .GreaterThan(0).WithMessage(x => localizer[Messages.Flight_EconomyPrice_Must_Be_Positive]);

            RuleFor(x => x.BasePremiumEconomyPrice)
                .GreaterThan(0).WithMessage(x => localizer[Messages.Flight_PremiumEconomyPrice_Must_Be_Positive])
                .GreaterThan(x => x.BaseEconomyPrice).WithMessage(x => localizer[Messages.Flight_PremiumEconomy_Must_Be_GreaterThan_Economy]);

            RuleFor(x => x.BaseBusinessPrice)
                .GreaterThan(0).WithMessage(x => localizer[Messages.Flight_BusinessPrice_Must_Be_Positive])
                .GreaterThan(x => x.BasePremiumEconomyPrice).WithMessage(x => localizer[Messages.Flight_Business_Must_Be_GreaterThan_PremiumEconomy]);

            RuleFor(x => x.BaseFirstClassPrice)
                .GreaterThan(0).WithMessage(x => localizer[Messages.Flight_FirstClassPrice_Must_Be_Positive])
                .GreaterThan(x => x.BaseBusinessPrice).WithMessage(x => localizer[Messages.Flight_FirstClass_Must_Be_GreaterThan_Business]);

            RuleFor(x => x.Currency)
                .IsInEnum().WithMessage(x => localizer[Messages.Flight_Please_Select_Currency]);

            RuleFor(x => x.Gate)
                .MaximumLength(10).WithMessage(x => localizer[Messages.Flight_Gate_Cannot_Be_GreaterThan_10Characters])
                .When(x => x.Gate != null);

            RuleFor(x => x.Terminal)
                .MaximumLength(10).WithMessage(x => localizer[Messages.Flight_Terminal_Cannot_Be_GreaterThan_10Characters])
                .When(x => x.Terminal != null);

            RuleFor(x => x.AircraftId)
                .NotEmpty().WithMessage(x => localizer[Messages.Flight_Please_Select_Aircraft]);

            RuleFor(x => x.AirlineId)
                .NotEmpty().WithMessage(x => localizer[Messages.Flight_Please_Select_Airline]);

            RuleFor(x => x.ScheduleId)
                .NotEmpty().WithMessage(x => localizer[Messages.Flight_Please_Select_Schedule]);
        }
    }
}
