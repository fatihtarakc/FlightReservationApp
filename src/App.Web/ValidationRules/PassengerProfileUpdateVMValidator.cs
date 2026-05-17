using App.Web.ViewModels.Passenger;
using FluentValidation;
namespace App.Web.ValidationRules
{
    public class PassengerProfileUpdateVMValidator : AbstractBaseValidator<PassengerProfileUpdateVM>
    {
        public PassengerProfileUpdateVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer[Messages.Val_Name_Required])
                .MinimumLength(2).WithMessage(localizer[Messages.Val_Name_TooShort])
                .MaximumLength(50).WithMessage(localizer[Messages.Val_Name_TooLong]);

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage(localizer[Messages.Val_Surname_Required])
                .MinimumLength(2).WithMessage(localizer[Messages.Val_Surname_TooShort])
                .MaximumLength(50).WithMessage(localizer[Messages.Val_Surname_TooLong]);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[Messages.Val_Email_Required])
                .EmailAddress().WithMessage(localizer[Messages.Val_Email_Invalid])
                .MaximumLength(50).WithMessage(localizer[Messages.Val_Email_Invalid]);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(localizer[Messages.Val_Phone_Required])
                .Matches(@"^\+[1-9]\d{7,14}$").WithMessage(localizer[Messages.Val_Phone_Format]);
        }
    }
}
