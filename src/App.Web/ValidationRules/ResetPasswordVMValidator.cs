
using App.Web.ViewModels.Account;
using FluentValidation;
namespace App.Web.ValidationRules
{
    public class ResetPasswordVMValidator : AbstractBaseValidator<ResetPasswordVM>
    {
        public ResetPasswordVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[Messages.Val_Email_Required])
                .EmailAddress().WithMessage(localizer[Messages.Val_Email_Invalid]);
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(localizer[Messages.Val_Code_Required])
                .Length(6).WithMessage(localizer[Messages.Val_Code_Length]);
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(localizer[Messages.Val_Password_Required])
                .MinimumLength(8).WithMessage(localizer[Messages.Val_Password_SignUp_TooShort])
                .Matches(@"[A-Z]").WithMessage(localizer[Messages.Val_Password_Uppercase])
                .Matches(@"[0-9]").WithMessage(localizer[Messages.Val_Password_Digit])
                .Matches(@"[^a-zA-Z0-9]").WithMessage(localizer[Messages.Val_Password_Special]);
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword).WithMessage(localizer[Messages.Val_ConfirmPassword_Match]);
        }
    }
}
