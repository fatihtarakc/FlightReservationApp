namespace App.Api.Validators
{
    public class AircraftUpdateDtoValidator : AbstractValidator<AircraftUpdateDto>
    {
        public AircraftUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.TailNumber)
                .NotEmpty().WithMessage(Messages.Aircraft_Please_Enter_TailNumber)
                .MinimumLength(2).WithMessage(Messages.Aircraft_TailNumber_Cannot_Be_LessThan_2Characters)
                .MaximumLength(10).WithMessage(Messages.Aircraft_TailNumber_Cannot_Be_GreaterThan_10Characters)
                .Matches(@"^[A-Z0-9-]+$").WithMessage(Messages.Aircraft_TailNumber_Format_Invalid);

            RuleFor(x => x.ManufactureYear)
                .InclusiveBetween(1950, DateTime.UtcNow.Year + 2)
                .WithMessage(Messages.Aircraft_ManufactureYear_Invalid);

            RuleFor(x => x.AircraftStatus)
                .IsInEnum().WithMessage(Messages.Aircraft_Please_Select_Status);

            RuleFor(x => x.AirlineId)
                .NotEmpty().WithMessage(Messages.Aircraft_Please_Select_Airline);

            RuleFor(x => x.ModelId)
                .NotEmpty().WithMessage(Messages.Aircraft_Please_Select_Model);
        }
    }
}
