using FluentValidation;
namespace App.Web.ValidationRules
{
    public class SignUpVMValidator : AbstractBaseValidator<SignUpVM>
    {
        public SignUpVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer[Messages.Val_Name_Required])
                .MinimumLength(2).WithMessage(localizer[Messages.Val_Name_TooShort])
                .MaximumLength(50).WithMessage(localizer[Messages.Val_Name_TooLong]);
            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage(localizer[Messages.Val_Surname_Required])
                .MinimumLength(2).WithMessage(localizer[Messages.Val_Surname_TooShort])
                .MaximumLength(50).WithMessage(localizer[Messages.Val_Surname_TooLong]);
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(localizer[Messages.Val_Username_Required])
                .MinimumLength(3).MaximumLength(30)
                .Matches(@"^[a-zA-Z0-9._-]+$").WithMessage(localizer[Messages.Val_Username_Format]);
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[Messages.Val_Email_Required])
                .EmailAddress().WithMessage(localizer[Messages.Val_Email_Invalid]);
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer[Messages.Val_Password_Required])
                .MinimumLength(8).WithMessage(localizer[Messages.Val_Password_SignUp_TooShort])
                .Matches(@"[A-Z]").WithMessage(localizer[Messages.Val_Password_Uppercase])
                .Matches(@"[a-z]").WithMessage(localizer[Messages.Val_Password_Lowercase])
                .Matches(@"[0-9]").WithMessage(localizer[Messages.Val_Password_Digit])
                .Matches(@"[^a-zA-Z0-9]").WithMessage(localizer[Messages.Val_Password_Special]);
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(localizer[Messages.Val_ConfirmPassword_Required])
                .Equal(x => x.Password).WithMessage(localizer[Messages.Val_ConfirmPassword_Match]);
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(localizer[Messages.Val_Phone_Required])
                .Matches(@"^\+[1-9]\d{7,14}$").WithMessage(localizer[Messages.Val_Phone_Format]);
            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage(localizer[Messages.Val_BirthDate_Required])
                .LessThan(DateTime.Today.AddYears(-18)).WithMessage(localizer[Messages.Val_BirthDate_MinAge])
                .GreaterThan(new DateTime(1900, 1, 1)).WithMessage(localizer[Messages.Val_BirthDate_Valid]);
        }
    }
}
