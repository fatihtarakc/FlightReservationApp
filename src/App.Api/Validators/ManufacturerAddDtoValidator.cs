namespace App.Api.Validators
{
    public class ManufacturerAddDtoValidator : AbstractValidator<ManufacturerAddDto>
    {
        public ManufacturerAddDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.Manufacturer_Please_Enter_Name)
                .MinimumLength(2).WithMessage(Messages.Manufacturer_Name_Cannot_Be_LessThan_2Characters)
                .MaximumLength(100).WithMessage(Messages.Manufacturer_Name_Cannot_Be_GreaterThan_100Characters);

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage(Messages.Manufacturer_Please_Enter_Country)
                .MinimumLength(2).WithMessage(Messages.Manufacturer_Country_Cannot_Be_LessThan_2Characters)
                .MaximumLength(100).WithMessage(Messages.Manufacturer_Country_Cannot_Be_GreaterThan_100Characters);
        }
    }
}
