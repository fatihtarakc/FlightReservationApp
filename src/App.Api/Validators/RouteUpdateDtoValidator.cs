namespace App.Api.Validators
{
    public class RouteUpdateDtoValidator : AbstractValidator<RouteUpdateDto>
    {
        public RouteUpdateDtoValidator()
        {
            RuleFor(x => x.DistanceKm)
                .GreaterThan(0).WithMessage(Messages.Route_DistanceKm_Must_Be_Positive);

            RuleFor(x => x.EstimatedDuration)
                .Must(d => d > TimeSpan.Zero).WithMessage(Messages.Route_EstimatedDuration_Must_Be_Positive);
        }
    }
}
