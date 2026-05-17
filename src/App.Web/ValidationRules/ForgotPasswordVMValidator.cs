using App.Web.ViewModels.Account;
using FluentValidation;
namespace App.Web.ValidationRules
{
    public class ForgotPasswordVMValidator : AbstractBaseValidator<ForgotPasswordVM>
    {
        public ForgotPasswordVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[Messages.Val_Email_Required])
                .EmailAddress().WithMessage(localizer[Messages.Val_Email_Invalid])
                .MaximumLength(50).WithMessage(localizer[Messages.Val_Email_Invalid]);
        }
    }
}
