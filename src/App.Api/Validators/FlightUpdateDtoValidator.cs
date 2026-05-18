namespace App.Api.Validators
{
    public class FlightUpdateDtoValidator : AbstractValidator<FlightUpdateDto>
    {
        public FlightUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.BaseEconomyPrice)
                .GreaterThan(0).WithMessage(Messages.Flight_EconomyPrice_Must_Be_Positive);

            RuleFor(x => x.BasePremiumEconomyPrice)
                .GreaterThan(0).WithMessage(Messages.Flight_PremiumEconomyPrice_Must_Be_Positive)
                .GreaterThan(x => x.BaseEconomyPrice).WithMessage(Messages.Flight_PremiumEconomy_Must_Be_GreaterThan_Economy);

            RuleFor(x => x.BaseBusinessPrice)
                .GreaterThan(0).WithMessage(Messages.Flight_BusinessPrice_Must_Be_Positive)
                .GreaterThan(x => x.BasePremiumEconomyPrice).WithMessage(Messages.Flight_Business_Must_Be_GreaterThan_PremiumEconomy);

            RuleFor(x => x.BaseFirstClassPrice)
                .GreaterThan(0).WithMessage(Messages.Flight_FirstClassPrice_Must_Be_Positive)
                .GreaterThan(x => x.BaseBusinessPrice).WithMessage(Messages.Flight_FirstClass_Must_Be_GreaterThan_Business);

            RuleFor(x => x.FlightStatus)
                .IsInEnum().WithMessage(Messages.Flight_Please_Select_Status);

            RuleFor(x => x.Gate)
                .MaximumLength(10).WithMessage(Messages.Flight_Gate_Cannot_Be_GreaterThan_10Characters)
                .When(x => x.Gate != null);

            RuleFor(x => x.Terminal)
                .MaximumLength(10).WithMessage(Messages.Flight_Terminal_Cannot_Be_GreaterThan_10Characters)
                .When(x => x.Terminal != null);

            RuleFor(x => x.CancellationReason)
                .MaximumLength(500).WithMessage(Messages.Flight_CancellationReason_Cannot_Be_GreaterThan_500Characters)
                .When(x => x.CancellationReason != null);
        }
    }
}
