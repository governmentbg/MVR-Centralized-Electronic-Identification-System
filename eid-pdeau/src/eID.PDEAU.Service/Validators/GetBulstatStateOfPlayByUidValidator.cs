using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetBulstatStateOfPlayByUidValidator : AbstractValidator<(Guid CorrelationId, string Uid)>
{
    public GetBulstatStateOfPlayByUidValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();

        RuleFor(r => r.Uid)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EikFormatIsValid(uid));
    }
}
