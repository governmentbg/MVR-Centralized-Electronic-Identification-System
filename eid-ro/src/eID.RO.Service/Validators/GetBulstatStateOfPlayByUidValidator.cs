using FluentValidation;

namespace eID.RO.Service.Validators;

internal class GetBulstatStateOfPlayByUidValidator : AbstractValidator<(Guid CorrelationId, string Uid)>
{
    public GetBulstatStateOfPlayByUidValidator()
    {
        RuleFor(r => r.CorrelationId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EikFormatIsValid(uid));
    }
}
