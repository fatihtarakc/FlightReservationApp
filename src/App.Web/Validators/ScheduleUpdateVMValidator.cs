using FluentValidation;
using Microsoft.Extensions.Localization;

namespace App.Web.Validators
{
    public class ScheduleUpdateVMValidator : AbstractValidator<ScheduleUpdateVM>
    {
        public ScheduleUpdateVMValidator(IStringLocalizer<SharedResources> localizer)
        {
            RuleFor(x => x.ValidFrom)
                .NotEmpty().WithMessage(_ => localizer[Messages.Val_Schedule_ValidFrom_Required]);

            RuleFor(x => x.ValidTo)
                .Must((vm, validTo) => validTo!.Value > vm.ValidFrom)
                .WithMessage(_ => localizer[Messages.Val_Schedule_ValidTo_AfterValidFrom])
                .When(x => x.ValidTo.HasValue);

            RuleFor(x => x.SelectedDays)
                .Must(days => days != null && days.Count > 0)
                .WithMessage(_ => localizer[Messages.Val_Schedule_Days_Required]);

            RuleFor(x => x.DepartureTime)
                .NotEmpty().WithMessage(_ => localizer[Messages.Val_Schedule_DepartureTime_Required])
                .Must(t => TimeSpan.TryParse(t, out var ts) && ts >= TimeSpan.Zero && ts < TimeSpan.FromDays(1))
                .WithMessage(_ => localizer[Messages.Val_Schedule_DepartureTime_Invalid])
                .When(x => !string.IsNullOrEmpty(x.DepartureTime));

            RuleFor(x => x.TimeZone)
                .NotEmpty().WithMessage(_ => localizer[Messages.Val_Schedule_TimeZone_Required])
                .MaximumLength(100).WithMessage(_ => localizer[Messages.Val_Schedule_TimeZone_MaxLength]);
        }
    }
}
