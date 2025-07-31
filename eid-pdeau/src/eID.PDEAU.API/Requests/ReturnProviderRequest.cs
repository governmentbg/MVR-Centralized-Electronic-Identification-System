using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class ReturnProviderRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new ReturnProviderRequestValidator();
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Return for correction action comment. Optional
    /// </summary>
    public string Comment { get; set; } = string.Empty;
}

internal class ReturnProviderRequestValidator : AbstractValidator<ReturnProviderRequest>
{
    public ReturnProviderRequestValidator()
    {
        RuleFor(r => r.ProviderId)
            .NotEmpty();

        RuleFor(r => r.Comment)
            .NotEmpty();
    }
}
