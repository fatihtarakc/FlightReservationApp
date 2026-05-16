using FluentValidation;
namespace App.Web.ValidationRules
{
    public abstract class AbstractBaseValidator<T> : AbstractValidator<T>
    {
        protected readonly IStringLocalizer<SharedResources> _localizer;

        protected AbstractBaseValidator(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }
    }
}
