using FluentValidation;

namespace App.Api.Validators
{
    public class FlightAddDtoValidator : AbstractValidator<FlightAddDto>
    {
        public FlightAddDtoValidator()
        {
            RuleFor(x => x.Number)
                .NotEmpty().WithMessage("Flight number is required.")
                .MaximumLength(10).WithMessage("Flight number cannot exceed 10 characters.");

            RuleFor(x => x.DepartureDateTime)
                .NotEmpty().WithMessage("Departure date/time is required.")
                .GreaterThan(DateTime.UtcNow).WithMessage("Departure date must be in the future.");

            RuleFor(x => x.ArrivalDateTime)
                .NotEmpty().WithMessage("Arrival date/time is required.")
                .GreaterThan(x => x.DepartureDateTime).WithMessage("Arrival must be after departure.");

            RuleFor(x => x.BaseEconomyPrice)
                .GreaterThan(0).WithMessage("Economy price must be greater than 0.");

            RuleFor(x => x.AircraftId)
                .NotEmpty().WithMessage("Aircraft is required.");

            RuleFor(x => x.AirlineId)
                .NotEmpty().WithMessage("Airline is required.");

            RuleFor(x => x.ScheduleId)
                .NotEmpty().WithMessage("Schedule is required.");
        }
    }
}
