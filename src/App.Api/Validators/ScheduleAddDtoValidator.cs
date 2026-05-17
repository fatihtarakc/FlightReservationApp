namespace App.Api.Validators
{
    public class ScheduleAddDtoValidator : AbstractValidator<ScheduleAddDto>
    {
        public ScheduleAddDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(Messages.Schedule_Please_Enter_Code)
                .MinimumLength(3).WithMessage(Messages.Schedule_Code_Cannot_Be_LessThan_3Characters)
                .MaximumLength(20).WithMessage(Messages.Schedule_Code_Cannot_Be_GreaterThan_20Characters)
                .Matches(@"^[A-Za-z0-9 .&-]+$").WithMessage(Messages.Schedule_Code_Format_Invalid);

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

            RuleFor(x => x.RouteId)
                .NotEmpty().WithMessage(Messages.Schedule_Please_Select_Route);
        }
    }
}
