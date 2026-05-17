using App.Web.ViewModels.Route;
using FluentValidation;
namespace App.Web.ValidationRules
{
    public class RouteAddVMValidator : AbstractBaseValidator<RouteAddVM>
    {
        public RouteAddVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.OriginAirportId)
                .NotEmpty().WithMessage(localizer[Messages.Val_OriginAirportId_Required]);

            RuleFor(x => x.DestinationAirportId)
                .NotEmpty().WithMessage(localizer[Messages.Val_DestinationAirportId_Required])
                .NotEqual(x => x.OriginAirportId).WithMessage(localizer[Messages.Val_SameAirport]);

            RuleFor(x => x.DistanceKm)
                .GreaterThan(0).WithMessage(localizer[Messages.Val_DistanceKm_Positive]);

            RuleFor(x => x.EstimatedDuration)
                .GreaterThan(0).WithMessage(localizer[Messages.Val_EstimatedDuration_Positive]);
        }
    }
}
