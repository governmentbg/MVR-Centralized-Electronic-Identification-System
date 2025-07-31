using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class ApproveProviderRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new ApproveProviderRequestValidator();
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Approve action comment. Optional
    /// </summary>
    public string Comment { get; set; } = string.Empty;
}

internal class ApproveProviderRequestValidator : AbstractValidator<ApproveProviderRequest>
{
    public ApproveProviderRequestValidator()
    {
        RuleFor(r => r.ProviderId).NotEmpty();
    }
}
