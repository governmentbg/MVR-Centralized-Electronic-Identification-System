using FluentValidation;

namespace eID.RO.Service.Validators;

internal class GetBulstatStateOfPlayByUidValidator : AbstractValidator<string>
{
    public GetBulstatStateOfPlayByUidValidator()
    {
        RuleFor(r => r)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(r => ValidatorHelpers.EikFormatIsValid(r));
    }
}
