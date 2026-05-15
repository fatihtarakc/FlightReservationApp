namespace App.Api.Validators
{
    public class SignInDtoValidator : AbstractValidator<SignInDto>
    {
        public SignInDtoValidator()
        {
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Email);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Messages.Account_Please_Enter_Your_Password);
        }
    }
}
