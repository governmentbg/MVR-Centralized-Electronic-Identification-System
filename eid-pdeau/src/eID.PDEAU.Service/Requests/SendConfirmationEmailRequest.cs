using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.Service.Requests;

public class SendConfirmationEmailRequest
{
    public Guid CorrelationId { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public Guid AdministratorPromotionId { get; set; }
}

public class SendConfirmationEmailRequestValidator : AbstractValidator<SendConfirmationEmailRequest>
{
    public SendConfirmationEmailRequestValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Uid).NotEmpty();
        RuleFor(r => r.UidType).NotEmpty();
        RuleFor(r => r.AdministratorPromotionId).NotEmpty();
    }
}
