namespace App.Api.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Name)
                .MinimumLength(2).WithMessage(Messages.Account_Name_Cannot_Be_LessThan_2Characters)
                .MaximumLength(50).WithMessage(Messages.Account_Name_Cannot_Be_GreaterThan_50Characters)
                .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage(Messages.Account_Digits_ForName_Cannot_Be_Used);

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Surname)
                .MinimumLength(2).WithMessage(Messages.Account_Surname_Cannot_Be_LessThan_2Characters)
                .MaximumLength(50).WithMessage(Messages.Account_Surname_Cannot_Be_GreaterThan_50Characters);

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Username)
                .MinimumLength(5).WithMessage(Messages.Account_Username_Cannot_Be_LessThan_5Characters)
                .MaximumLength(30).WithMessage(Messages.Account_Username_Cannot_Be_GreaterThan_30Characters);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Email)
                .EmailAddress().WithMessage(Messages.Account_Please_Enter_Your_Email_With_Correct_Format)
                .MinimumLength(5).WithMessage(Messages.Account_Email_Cannot_Be_LessThan_5Characters)
                .MaximumLength(100).WithMessage(Messages.Account_Email_Cannot_Be_GreaterThan_100Characters);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Password)
                .MinimumLength(8).WithMessage(Messages.Account_Password_Cannot_Be_LessThan_8Characters)
                .MaximumLength(50).WithMessage(Messages.Account_Password_Cannot_Be_GreaterThan_50Characters)
                .Matches(@"[A-Z]").WithMessage(Messages.Account_Password_Must_Include_1UpperLetter_AtLeast)
                .Matches(@"[a-z]").WithMessage(Messages.Account_Password_Must_Include_1LowerLetter_AtLeast)
                .Matches(@"[0-9]").WithMessage(Messages.Account_Password_Must_Include_1Digit_AtLeast)
                .Matches(@"[!@#$%^&*(),.?""':{}|<>]").WithMessage(Messages.Account_Password_Must_Include_1Symbol_AtLeast);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_PhoneNumber)
                .Matches(@"^\+?[1-9]\d{9,14}$").WithMessage(Messages.Account_PhoneNumber_Is_Invalid);

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Birthdate)
                .Must(d => DateTime.UtcNow.Year - d.Year >= 12)
                .WithMessage(Messages.Account_Birthdate_Age_Cannot_Be_LessThan_12YearsOld);

            RuleFor(x => x.PreferredNotificationChannel)
                .IsInEnum().WithMessage(Messages.Account_Please_Select_NotificationPreference);
        }
    }
}
