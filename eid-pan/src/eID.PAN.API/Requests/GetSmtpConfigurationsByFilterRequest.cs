using FluentValidation;

namespace eID.PAN.API.Requests;

public class GetSmtpConfigurationsByFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetSmtpConfigurationsByFilterRequestValidator();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

public class GetSmtpConfigurationsByFilterRequestValidator : AbstractValidator<GetSmtpConfigurationsByFilterRequest>
{
    public GetSmtpConfigurationsByFilterRequestValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
    }
}

