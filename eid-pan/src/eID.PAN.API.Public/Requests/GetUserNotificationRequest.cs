using FluentValidation;

namespace eID.PAN.API.Public.Requests;

public class GetUserNotificationRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetUserNotificationRequestValidator();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string SystemName { get; set; } = string.Empty;
}

public class GetUserNotificationRequestValidator : AbstractValidator<GetUserNotificationRequest>
{
    public GetUserNotificationRequestValidator() 
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    }
}
