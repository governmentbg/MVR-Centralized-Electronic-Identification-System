using eID.PUN.Contracts.Commands;
using FluentValidation;

namespace eID.PUN.Service.Validators;

public class RegisterCarrierValidator : AbstractValidator<RegisterCarrier>
{
    public RegisterCarrierValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.SerialNumber).NotEmpty();
        RuleFor(r => r.Type).NotEmpty().MaximumLength(100);
        RuleFor(r => r.CertificateId).NotEmpty();
        RuleFor(r => r.EId).NotEmpty();
    }
}
