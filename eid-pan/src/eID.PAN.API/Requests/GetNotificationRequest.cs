using eID.PAN.Contracts.Enums;
using FluentValidation;

namespace eID.PAN.API.Requests;

public class GetNotificationRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetNotificationRequestValidator();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string SystemName { get; set; } = string.Empty;
    public bool IncludeDeleted { get; set; } = false;
    public RegisteredSystemState RegisteredSystemState { get; set; }
}

public class GetNotificationRequestValidator : AbstractValidator<GetNotificationRequest>
{
    public GetNotificationRequestValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
        RuleFor(r => r.RegisteredSystemState).IsInEnum();
    }
}
