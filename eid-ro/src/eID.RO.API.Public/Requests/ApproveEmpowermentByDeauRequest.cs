using FluentValidation;

namespace eID.RO.API.Public.Requests;

/// <summary>
/// Approve Unconfirmed empowerment by DEAU
/// </summary>
public class ApproveEmpowermentByDeauRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new ApproveEmpowermentByDeauRequestValidator();
    /// <summary>
    /// EmpowermentId : the unconfirmed empowerment to approve
    /// </summary>
    public Guid EmpowermentId { get; set; }
}

public class ApproveEmpowermentByDeauRequestValidator : AbstractValidator<ApproveEmpowermentByDeauRequest>
{
    public ApproveEmpowermentByDeauRequestValidator()
    {
        RuleFor(x => x.EmpowermentId).NotEmpty();
    }
}
