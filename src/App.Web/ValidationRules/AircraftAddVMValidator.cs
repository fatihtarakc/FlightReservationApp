using App.Web.ViewModels.Aircraft;
using FluentValidation;

namespace App.Web.ValidationRules
{
    public class AircraftAddVMValidator : AbstractBaseValidator<AircraftAddVM>
    {
        public AircraftAddVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.TailNumber)
                .NotEmpty().WithMessage(localizer[Messages.Val_TailNumber_Required]);
            RuleFor(x => x.ManufactureYear)
                .InclusiveBetween(1950, 2030).WithMessage(localizer[Messages.Val_ManufactureYear_Range]);
            RuleFor(x => x.AirlineId)
                .NotEmpty().WithMessage(localizer[Messages.Val_AirlineId_Required]);
            RuleFor(x => x.ModelId)
                .NotEmpty().WithMessage(localizer[Messages.Val_ModelId_Required]);
        }
    }
}
