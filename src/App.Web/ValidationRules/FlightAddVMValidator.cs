using App.Web.Resources;
using App.Web.ViewModels.Flight;
using FluentValidation;
namespace App.Web.ValidationRules
{
    public class FlightAddVMValidator : AbstractBaseValidator<FlightAddVM>
    {
        public FlightAddVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.FlightNumber)
                .NotEmpty().WithMessage(localizer[Messages.Val_FlightNumber_Required])
                .MaximumLength(10).WithMessage(localizer[Messages.Val_FlightNumber_MaxLength]);
            RuleFor(x => x.DepartureTime)
                .GreaterThan(DateTime.Now).WithMessage(localizer[Messages.Val_DepartureTime_Future]);
            RuleFor(x => x.ArrivalTime)
                .GreaterThan(x => x.DepartureTime).WithMessage(localizer[Messages.Val_ArrivalTime_AfterDeparture]);
            RuleFor(x => x.EconomyPrice)
                .GreaterThan(0).WithMessage(localizer[Messages.Val_EconomyPrice_Positive]);
            RuleFor(x => x.AircraftId)
                .NotEmpty().WithMessage(localizer[Messages.Val_AircraftId_Required]);
            RuleFor(x => x.RouteId)
                .NotEmpty().WithMessage(localizer[Messages.Val_RouteId_Required]);
        }
    }
}
