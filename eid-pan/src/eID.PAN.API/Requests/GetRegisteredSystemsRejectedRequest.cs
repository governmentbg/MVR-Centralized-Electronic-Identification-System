using FluentValidation;

namespace eID.PAN.API.Requests;

public class GetRegisteredSystemsRejectedRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetRegisteredSystemsRejectedValidator();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

public class GetRegisteredSystemsRejectedValidator : AbstractValidator<GetRegisteredSystemsRejectedRequest>
{
    public GetRegisteredSystemsRejectedValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    }
}
