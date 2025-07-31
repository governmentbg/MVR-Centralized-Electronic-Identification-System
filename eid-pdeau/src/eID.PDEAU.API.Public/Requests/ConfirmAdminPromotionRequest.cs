using FluentValidation;

namespace eID.PDEAU.API.Public.Requests;

public class ConfirmAdminPromotionRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new ConfirmAdminPromotionRequesttValidator();
    public Guid AdministratorPromotionId { get; set; }
}

public class ConfirmAdminPromotionRequesttValidator : AbstractValidator<ConfirmAdminPromotionRequest>
{
    public ConfirmAdminPromotionRequesttValidator()
    {
        RuleFor(r => r.AdministratorPromotionId).NotEmpty();
    }
}
