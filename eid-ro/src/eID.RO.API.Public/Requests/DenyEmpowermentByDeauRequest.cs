using FluentValidation;

namespace eID.RO.API.Public.Requests;

/// <summary>
/// Deny Unconfirmed or Active empowerment by DEAU
/// </summary>
public class DenyEmpowermentByDeauRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new DenyEmpowermentByDeauRequestValidator();
    /// <summary>
    /// EmpowermentId : the unconfirmed empowerment to deny
    /// </summary>
    public Guid EmpowermentId { get; set; }
    /// <summary>
    /// The reason for denying the empowerment application by DEAU
    /// </summary>
    public string DenialReasonComment { get; set; } = string.Empty;
}

public class DenyEmpowermentByDeauRequestValidator : AbstractValidator<DenyEmpowermentByDeauRequest>
{
    public DenyEmpowermentByDeauRequestValidator()
    {
        RuleFor(x => x.EmpowermentId).NotEmpty();
        RuleFor(x => x.DenialReasonComment).NotEmpty();
    }
}
