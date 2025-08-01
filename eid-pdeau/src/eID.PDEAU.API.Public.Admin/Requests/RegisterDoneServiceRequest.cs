using FluentValidation;

namespace eID.PDEAU.API.Public.Admin.Requests;

public class RegisterDoneServiceRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RegisterDoneServiceRequestValidator();

    public Guid ProviderId { get; set; }
    public string ServiceName { get; set; }
    public int Count { get; set; } = 1;
}

public class RegisterDoneServiceRequestValidator : AbstractValidator<RegisterDoneServiceRequest>
{
    public RegisterDoneServiceRequestValidator()
    {
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.ServiceName).NotEmpty().MaximumLength(1024);
        RuleFor(r => r.Count).GreaterThanOrEqualTo(1);
    }
}

public class RegisterDoneServicePayload
{
    /// <summary>
    /// Name of the service to be registered. Mandatory.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;
    
    /// <summary>
    /// How many times the service was used. Default 1.
    /// </summary>
    public int Count { get; set; } = 1;
}
