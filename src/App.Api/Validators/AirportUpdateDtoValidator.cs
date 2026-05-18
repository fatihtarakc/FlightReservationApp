namespace App.Api.Validators
{
    public class AirportUpdateDtoValidator : AbstractValidator<AirportUpdateDto>
    {
        public AirportUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.Airport_Please_Enter_Name)
                .MaximumLength(150).WithMessage(Messages.Airport_Name_Cannot_Be_GreaterThan_150Characters);

            RuleFor(x => x.IataCode)
                .NotEmpty().WithMessage(Messages.Airport_Please_Enter_IataCode)
                .Length(3).WithMessage(Messages.Airport_IataCode_Must_Be_3Characters)
                .Matches(@"^[A-Z]{3}$").WithMessage(Messages.Airport_IataCode_Format_Invalid);

            RuleFor(x => x.IcaoCode)
                .NotEmpty().WithMessage(Messages.Airport_Please_Enter_IcaoCode)
                .Length(4).WithMessage(Messages.Airport_IcaoCode_Must_Be_4Characters)
                .Matches(@"^[A-Z]{4}$").WithMessage(Messages.Airport_IcaoCode_Format_Invalid);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage(Messages.Airport_Please_Enter_City)
                .MaximumLength(100).WithMessage(Messages.Airport_City_Cannot_Be_GreaterThan_100Characters);

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage(Messages.Airport_Please_Enter_Country)
                .MaximumLength(100).WithMessage(Messages.Airport_Country_Cannot_Be_GreaterThan_100Characters);

            RuleFor(x => x.TimeZone)
                .NotEmpty().WithMessage(Messages.Airport_Please_Enter_TimeZone);

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage(Messages.Airport_Latitude_Invalid);

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage(Messages.Airport_Longitude_Invalid);
        }
    }
}
