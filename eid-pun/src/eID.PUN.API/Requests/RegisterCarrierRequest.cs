using FluentValidation;

namespace eID.PUN.API.Requests;

public class RegisterCarrierRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RegisterCarrierRequestValidator();
    public string SerialNumber { get; set; }
    public string Type { get; set; }
    public Guid CertificateId { get; set; }
    public Guid EId { get; set; }
}

internal class RegisterCarrierRequestValidator : AbstractValidator<RegisterCarrierRequest>
{
    public RegisterCarrierRequestValidator()
    {
        RuleFor(r => r.SerialNumber).NotEmpty();
        RuleFor(r => r.Type).NotEmpty().MaximumLength(100);
        RuleFor(r => r.CertificateId).NotEmpty();
        RuleFor(r => r.EId).NotEmpty();
    }
}
