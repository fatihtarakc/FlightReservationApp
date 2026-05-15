using App.Web.Resources;
using App.Web.ViewModels.Booking;
using FluentValidation;
namespace App.Web.ValidationRules
{
    public class BookingAddVMValidator : AbstractBaseValidator<BookingAddVM>
    {
        public BookingAddVMValidator(IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            RuleFor(x => x.FlightId)
                .NotEmpty().WithMessage(localizer[Messages.Val_FlightId_Required]);
            RuleFor(x => x.SeatId)
                .NotEmpty().WithMessage(localizer[Messages.Val_SeatId_Required]);
        }
    }
}
