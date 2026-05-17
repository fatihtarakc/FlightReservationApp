using App.Web.ViewModels.Aircraft;
using FluentValidation;

namespace App.Web.ValidationRules
{
    public class AircraftUpdateVMValidator : AbstractBaseValidator<AircraftUpdateVM>
    {
        public AircraftUpdateVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.TailNumber)
                .NotEmpty().WithMessage(localizer[Messages.Val_TailNumber_Required])
                .MinimumLength(2).WithMessage(localizer[Messages.Val_TailNumber_MinLength])
                .MaximumLength(10).WithMessage(localizer[Messages.Val_TailNumber_Required])
                .Matches(@"^[A-Z0-9-]+$").WithMessage(localizer[Messages.Val_TailNumber_Format]);
            RuleFor(x => x.ManufactureYear)
                .InclusiveBetween(1950, 2030).WithMessage(localizer[Messages.Val_ManufactureYear_Range]);
            RuleFor(x => x.AirlineId)
                .NotEmpty().WithMessage(localizer[Messages.Val_AirlineId_Required]);
            RuleFor(x => x.ModelId)
                .NotEmpty().WithMessage(localizer[Messages.Val_ModelId_Required]);
        }
    }
}
