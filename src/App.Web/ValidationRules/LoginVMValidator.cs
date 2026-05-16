
using App.Web.ViewModels.Account;
using FluentValidation;
namespace App.Web.ValidationRules
{
    public class LoginVMValidator : AbstractBaseValidator<LoginVM>
    {
        public LoginVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty().WithMessage(localizer[Messages.Val_UsernameOrEmail_Required])
                .MinimumLength(3).WithMessage(localizer[Messages.Val_UsernameOrEmail_MinLength]);
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer[Messages.Val_Password_Required])
                .MinimumLength(6).WithMessage(localizer[Messages.Val_Password_TooShort]);
        }
    }
}
