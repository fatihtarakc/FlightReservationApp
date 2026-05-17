namespace App.Api.Validators
{
    public class VerifyEmailDtoValidator : AbstractValidator<VerifyEmailDto>
    {
        public VerifyEmailDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Email)
                .EmailAddress().WithMessage(Messages.Account_Please_Enter_Your_Email_With_Correct_Format);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_VerificationCode)
                .Length(6).WithMessage(Messages.Account_VerificationCode_Is_Invalid);
        }
    }
}
