using eID.PAN.Contracts.Enums;
using FluentValidation;

namespace eID.PAN.API.Requests;

public class GetSystemsByFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetSystemsByFilterRequestValidator();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public RegisteredSystemState RegisteredSystemState { get; set; }
}

public class GetSystemsByFilterRequestValidator : AbstractValidator<GetSystemsByFilterRequest>
{
    public GetSystemsByFilterRequestValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
        RuleFor(r => r.RegisteredSystemState).IsInEnum();
    }
}
