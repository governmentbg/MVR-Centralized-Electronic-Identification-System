using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class DenyServicePayload : IValidatableRequest
{
    public virtual IValidator GetValidator() => new DenyServicePayloadValidator();
    public string DenialReason { get; set; }
}

internal class DenyServicePayloadValidator : AbstractValidator<DenyServicePayload>
{
    public DenyServicePayloadValidator()
    {
        RuleFor(r => r.DenialReason).NotEmpty().MaximumLength(Contracts.Constants.ProviderService.DenialReasonMaxLength);
    }
}
