namespace App.Api.Validators
{
    public class AirlineAddDtoValidator : AbstractValidator<AirlineAddDto>
    {
        public AirlineAddDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Airline name is required.")
                .MaximumLength(100).WithMessage("Airline name cannot exceed 100 characters.");

            RuleFor(x => x.IataCode)
                .NotEmpty().WithMessage("IATA code is required.")
                .Length(2).WithMessage("IATA code must be exactly 2 characters.");

            RuleFor(x => x.IcaoCode)
                .NotEmpty().WithMessage("ICAO code is required.")
                .Length(3).WithMessage("ICAO code must be exactly 3 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");
        }
    }
}
