using FluentValidation;

namespace eID.RO.Service.Requests;

public class GetExpiringEmpowermentsRequest : IValidatableRequest
{
    public Guid CorrelationId { get; set; }
    /// <summary>
    /// Amount of days that are left from Empowerment statement's active time, in which all parties will get notified about the incoming expiration
    /// </summary>
    public int DaysUntilExpiration { get; set; }
    public virtual IValidator GetValidator() => new GetExpiringEmpowermentsRequestValidator();
}


public class GetExpiringEmpowermentsRequestValidator : AbstractValidator<GetExpiringEmpowermentsRequest>
{
    public GetExpiringEmpowermentsRequestValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(x => x.DaysUntilExpiration).NotEmpty();
    }
}
