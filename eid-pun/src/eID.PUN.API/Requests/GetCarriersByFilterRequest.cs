using FluentValidation;

namespace eID.PUN.API.Requests;

/// <summary>
/// Get carrier by filter
/// </summary>
public class GetCarriersByFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetCarriersByFilterRequestValidator();
    public string SerialNumber { get; set; } = string.Empty;
    public Guid EId { get; set; }
    public Guid CertificateId { get; set; }
    public string Type { get; set; } = string.Empty;
}

internal class GetCarriersByFilterRequestValidator : AbstractValidator<GetCarriersByFilterRequest>
{
    public GetCarriersByFilterRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

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
