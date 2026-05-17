namespace App.Api.Validators
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Email)
                .EmailAddress().WithMessage(Messages.Account_Please_Enter_Your_Email_With_Correct_Format);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_VerificationCode)
                .Length(6).WithMessage(Messages.Account_VerificationCode_Is_Invalid);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Password)
                .MinimumLength(8).WithMessage(Messages.Account_Password_Cannot_Be_LessThan_8Characters)
                .MaximumLength(50).WithMessage(Messages.Account_Password_Cannot_Be_GreaterThan_50Characters)
                .Matches(@"[A-Z]").WithMessage(Messages.Account_Password_Must_Include_1UpperLetter_AtLeast)
                .Matches(@"[a-z]").WithMessage(Messages.Account_Password_Must_Include_1LowerLetter_AtLeast)
                .Matches(@"[0-9]").WithMessage(Messages.Account_Password_Must_Include_1Digit_AtLeast)
                .Matches(@"[!@#$%^&*(),.?"":{}|<>]").WithMessage(Messages.Account_Password_Must_Include_1Symbol_AtLeast);
        }
    }
}
