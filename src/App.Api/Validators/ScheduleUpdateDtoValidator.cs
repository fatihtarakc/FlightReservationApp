namespace App.Api.Validators
{
    public class ScheduleUpdateDtoValidator : AbstractValidator<ScheduleUpdateDto>
    {
        public ScheduleUpdateDtoValidator()
        {
            RuleFor(x => x.ValidFrom)
                .NotEmpty().WithMessage(Messages.Schedule_ValidFrom_Required);

            RuleFor(x => x.ValidTo)
                .Must((dto, validTo) => validTo > dto.ValidFrom)
                .WithMessage(Messages.Schedule_ValidTo_Must_Be_After_ValidFrom)
                .When(x => x.ValidTo.HasValue);

            RuleFor(x => x.DaysOfWeek)
                .Must(d => d != DaysOfWeek.None).WithMessage(Messages.Schedule_Please_Select_DaysOfWeek);

            RuleFor(x => x.DepartureTime)
                .Must(t => t >= TimeSpan.Zero && t < TimeSpan.FromDays(1))
                .WithMessage(Messages.Schedule_DepartureTime_Required);

            RuleFor(x => x.TimeZone)
                .NotEmpty().WithMessage(Messages.Schedule_Please_Enter_TimeZone)
                .MaximumLength(100).WithMessage(Messages.Schedule_TimeZone_Cannot_Be_GreaterThan_100Characters);
        }
    }
}
