using FluentValidation;

namespace eID.PAN.API.Requests;

public class GetSmtpConfigurationByIdRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetSmtpConfigurationByIdRequestValidator();
    public string Id { get; set; }
}

public class GetSmtpConfigurationByIdRequestValidator : AbstractValidator<GetSmtpConfigurationByIdRequest>
{
    public GetSmtpConfigurationByIdRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
