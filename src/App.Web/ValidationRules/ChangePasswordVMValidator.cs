using App.Web.ViewModels.Passenger;
using FluentValidation;
namespace App.Web.ValidationRules
{
    public class ChangePasswordVMValidator : AbstractBaseValidator<ChangePasswordVM>
    {
        public ChangePasswordVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(localizer[Messages.Val_CurrentPassword_Required]);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(localizer[Messages.Val_Password_Required])
                .MinimumLength(8).WithMessage(localizer[Messages.Val_Password_SignUp_TooShort])
                .Matches(@"[A-Z]").WithMessage(localizer[Messages.Val_Password_Uppercase])
                .Matches(@"[0-9]").WithMessage(localizer[Messages.Val_Password_Digit])
                .Matches(@"[^a-zA-Z0-9]").WithMessage(localizer[Messages.Val_Password_Special]);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(localizer[Messages.Val_ConfirmPassword_Required])
                .Equal(x => x.NewPassword).WithMessage(localizer[Messages.Val_ConfirmPassword_Match]);
        }
    }
}
