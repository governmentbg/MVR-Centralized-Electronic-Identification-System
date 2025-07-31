using eID.PUN.Contracts.Commands;
using FluentValidation;

namespace eID.PUN.Service.Validators;

public class GetCarriersByFilterValidator : AbstractValidator<GetCarriersByFilter>
{
    public GetCarriersByFilterValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(m => m)
            .Must(m =>
                !string.IsNullOrWhiteSpace(m.Type) ||
                !string.IsNullOrWhiteSpace(m.SerialNumber) ||
                m.EId != Guid.Empty ||
                m.CertificateId != Guid.Empty
            )
            .WithMessage("At least one of Type, SerialNumber, EId or CertificateId is required.");
    }
}
