namespace App.Api.Validators
{
    public class RouteAddDtoValidator : AbstractValidator<RouteAddDto>
    {
        public RouteAddDtoValidator()
        {
            RuleFor(x => x.DepartureAirportId)
                .NotEmpty().WithMessage(Messages.Route_Please_Select_DepartureAirport);

            RuleFor(x => x.ArrivalAirportId)
                .NotEmpty().WithMessage(Messages.Route_Please_Select_ArrivalAirport)
                .NotEqual(x => x.DepartureAirportId).WithMessage(Messages.Route_SameAirport_Invalid);

            RuleFor(x => x.DistanceKm)
                .GreaterThan(0).WithMessage(Messages.Route_DistanceKm_Must_Be_Positive);

            RuleFor(x => x.EstimatedDuration)
                .Must(d => d > TimeSpan.Zero).WithMessage(Messages.Route_EstimatedDuration_Must_Be_Positive);
        }
    }
}
