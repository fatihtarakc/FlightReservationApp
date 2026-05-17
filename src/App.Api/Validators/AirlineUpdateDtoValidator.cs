namespace App.Api.Validators
{
    public class AirlineUpdateDtoValidator : AbstractValidator<AirlineUpdateDto>
    {
        public AirlineUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.Airline_Please_Enter_Name)
                .MinimumLength(2).WithMessage(Messages.Airline_Name_Cannot_Be_LessThan_2Characters)
                .MaximumLength(50).WithMessage(Messages.Airline_Name_Cannot_Be_GreaterThan_50Characters)
                .Matches(@"^[A-Za-z0-9 &-]+$").WithMessage(Messages.Airline_Name_Format_Invalid);

            RuleFor(x => x.IataCode)
                .NotEmpty().WithMessage(Messages.Airline_Please_Enter_IataCode)
                .Length(2).WithMessage(Messages.Airline_IataCode_Must_Be_2Characters)
                .Matches(@"^[A-Z0-9]{2}$").WithMessage(Messages.Airline_IataCode_Format_Invalid);

            RuleFor(x => x.IcaoCode)
                .NotEmpty().WithMessage(Messages.Airline_Please_Enter_IcaoCode)
                .Length(3).WithMessage(Messages.Airline_IcaoCode_Must_Be_3Characters)
                .Matches(@"^[A-Z]{3}$").WithMessage(Messages.Airline_IcaoCode_Format_Invalid);

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage(Messages.Airline_Please_Enter_Country)
                .MaximumLength(50).WithMessage(Messages.Airline_Country_Cannot_Be_GreaterThan_50Characters);
        }
    }
}
