using FluentValidation;

namespace eID.PDEAU.Service.Requests;

public class CheckProviderDetailsRequest
{
    public string ProviderName { get; set; } = string.Empty;
}

public class CheckProviderDetailsRequestValidator : AbstractValidator<CheckProviderDetailsRequest>
{
    public CheckProviderDetailsRequestValidator()
    {
        RuleFor(r => r.ProviderName).NotEmpty();
    }
}
