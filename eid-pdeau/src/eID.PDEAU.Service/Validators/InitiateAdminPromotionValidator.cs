using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class InitiateAdminPromotionValidator : AbstractValidator<InitiateAdminPromotion>
{
    public InitiateAdminPromotionValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.IssuerUid).NotEmpty();
        RuleFor(r => r.IssuerUidType).NotEmpty().IsInEnum();
        RuleFor(r => r.IssuerName).NotEmpty();
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
    }
}
