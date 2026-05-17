using App.Web.ViewModels.Airport;
using FluentValidation;
namespace App.Web.ValidationRules
{
    public class AirportAddVMValidator : AbstractBaseValidator<AirportAddVM>
    {
        public AirportAddVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.IataCode)
                .NotEmpty().WithMessage(localizer[Messages.Val_IataCode_Required])
                .Length(3).WithMessage(localizer[Messages.Val_IataCode_Format])
                .Matches(@"^[A-Z]{3}$").WithMessage(localizer[Messages.Val_IataCode_Format]);

            RuleFor(x => x.IcaoCode)
                .Length(4).When(x => !string.IsNullOrEmpty(x.IcaoCode))
                .WithMessage(localizer[Messages.Val_IcaoCode_Format])
                .Matches(@"^[A-Z]{4}$").When(x => !string.IsNullOrEmpty(x.IcaoCode))
                .WithMessage(localizer[Messages.Val_IcaoCode_Format]);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer[Messages.Val_AirportName_Required])
                .MinimumLength(3).WithMessage(localizer[Messages.Val_AirportName_Required]);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage(localizer[Messages.Val_City_Required])
                .MinimumLength(2).WithMessage(localizer[Messages.Val_City_Required]);

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage(localizer[Messages.Val_Country_Required])
                .MinimumLength(2).WithMessage(localizer[Messages.Val_Country_Required]);

            RuleFor(x => x.Timezone)
                .NotEmpty().WithMessage(localizer[Messages.Val_Timezone_Required]);
        }
    }
}
