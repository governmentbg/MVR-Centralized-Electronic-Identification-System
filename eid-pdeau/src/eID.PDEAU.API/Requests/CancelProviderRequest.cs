using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class CancelProviderRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new CancelProviderRequestValidator();
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Cancel action comment. Optional
    /// </summary>
    public string Comment { get; set; } = string.Empty;
}

internal class CancelProviderRequestValidator : AbstractValidator<CancelProviderRequest>
{
    public CancelProviderRequestValidator()
    {
        RuleFor(r => r.ProviderId)
            .NotEmpty();
        
        RuleFor(r => r.Comment)
            .NotEmpty();
    }
}
