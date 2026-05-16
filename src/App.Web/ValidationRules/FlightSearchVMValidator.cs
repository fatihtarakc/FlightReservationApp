
using App.Web.ViewModels.Flight;
using FluentValidation;
namespace App.Web.ValidationRules
{
    public class FlightSearchVMValidator : AbstractBaseValidator<FlightSearchVM>
    {
        public FlightSearchVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.DepartureIata)
                .NotEmpty().WithMessage(localizer[Messages.Val_DepartureIata_Required])
                .Length(3).WithMessage(localizer[Messages.Val_IATA_Length]);
            RuleFor(x => x.ArrivalIata)
                .NotEmpty().WithMessage(localizer[Messages.Val_ArrivalIata_Required])
                .Length(3).WithMessage(localizer[Messages.Val_IATA_Length])
                .NotEqual(x => x.DepartureIata).WithMessage(localizer[Messages.Val_IATA_Different]);
            RuleFor(x => x.DepartureDate)
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage(localizer[Messages.Val_Date_NotPast]);
            RuleFor(x => x.Passengers)
                .InclusiveBetween(1, 9).WithMessage(localizer[Messages.Val_Passengers_Range]);
        }
    }
}
