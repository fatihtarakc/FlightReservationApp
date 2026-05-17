namespace App.Api.Validators
{
    public class ModelAddDtoValidator : AbstractValidator<ModelAddDto>
    {
        public ModelAddDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.Model_Please_Enter_Name)
                .MinimumLength(2).WithMessage(Messages.Model_Name_Cannot_Be_LessThan_2Characters)
                .MaximumLength(100).WithMessage(Messages.Model_Name_Cannot_Be_GreaterThan_100Characters);

            RuleFor(x => x.BodyType)
                .IsInEnum().WithMessage(Messages.Model_Please_Select_BodyType);

            RuleFor(x => x.MaxPassengerCapacity)
                .InclusiveBetween(1, 1000).WithMessage(Messages.Model_MaxPassengerCapacity_Invalid);

            RuleFor(x => x.EconomySeats)
                .GreaterThanOrEqualTo(0).WithMessage(Messages.Model_Seats_Cannot_Be_Negative);

            RuleFor(x => x.PremiumEconomySeats)
                .GreaterThanOrEqualTo(0).WithMessage(Messages.Model_Seats_Cannot_Be_Negative);

            RuleFor(x => x.BusinessSeats)
                .GreaterThanOrEqualTo(0).WithMessage(Messages.Model_Seats_Cannot_Be_Negative);

            RuleFor(x => x.FirstClassSeats)
                .GreaterThanOrEqualTo(0).WithMessage(Messages.Model_Seats_Cannot_Be_Negative);

            RuleFor(x => x.MaxRangeKm)
                .GreaterThan(0).WithMessage(Messages.Model_MaxRangeKm_Must_Be_Positive);

            RuleFor(x => x.ManufacturerId)
                .NotEmpty().WithMessage(Messages.Model_Please_Select_Manufacturer);
        }
    }
}
