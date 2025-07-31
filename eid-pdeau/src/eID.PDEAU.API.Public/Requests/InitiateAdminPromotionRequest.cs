using FluentValidation;

namespace eID.PDEAU.API.Public.Requests;

public class InitiateAdminPromotionRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new InitiateAdminPromotionRequestValidator();
    public Guid ProviderId { get; set; }
    public Guid UserId { get; set; }
}

public class InitiateAdminPromotionRequestValidator : AbstractValidator<InitiateAdminPromotionRequest>
{
    public InitiateAdminPromotionRequestValidator()
    {
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
    }
}
