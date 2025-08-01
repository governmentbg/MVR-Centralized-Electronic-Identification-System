using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class ConfirmAdminPromotionValidator : AbstractValidator<ConfirmAdminPromotion>
{
    public ConfirmAdminPromotionValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.AdministratorPromotionId).NotEmpty();
    }
}
