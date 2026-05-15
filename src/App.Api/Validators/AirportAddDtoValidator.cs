namespace App.Api.Validators
{
    public class AirportAddDtoValidator : AbstractValidator<AirportAddDto>
    {
        public AirportAddDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Airport name is required.")
                .MaximumLength(150).WithMessage("Airport name cannot exceed 150 characters.");

            RuleFor(x => x.IataCode)
                .NotEmpty().WithMessage("IATA code is required.")
                .Length(3).WithMessage("IATA code must be exactly 3 characters.");

            RuleFor(x => x.IcaoCode)
                .NotEmpty().WithMessage("ICAO code is required.")
                .Length(4).WithMessage("ICAO code must be exactly 4 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

            RuleFor(x => x.TimeZone)
                .NotEmpty().WithMessage("Timezone is required.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        }
    }
}
