using eID.PUN.Contracts.Commands;
using FluentValidation;

namespace eID.PUN.Service.Validators;

public class GetCarriersByValidator : AbstractValidator<GetCarriersBy>
{
    public GetCarriersByValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleFor(r => r.CorrelationId).NotEmpty();

        When(r => string.IsNullOrEmpty(r.SerialNumber) && r.EId == Guid.Empty, () => {
            RuleFor(m => m.CertificateId)
                .NotEmpty()
                .WithMessage("At least one of SerialNumber, EId or CertificateId is required");
        });

        When(r => r.EId == Guid.Empty && r.CertificateId == Guid.Empty, () => {
            RuleFor(m => m.SerialNumber)
                .NotEmpty()
                .WithMessage("At least one of SerialNumber, EId or CertificateId is required");
        });

        When(r => string.IsNullOrEmpty(r.SerialNumber) && r.CertificateId == Guid.Empty, () => {
            RuleFor(m => m.EId)
                .NotEmpty()
                .WithMessage("At least one of SerialNumber, EId or CertificateId is required");
        });
    }
}
