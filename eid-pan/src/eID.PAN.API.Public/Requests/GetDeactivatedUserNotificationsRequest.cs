using FluentValidation;

namespace eID.PAN.API.Public.Requests;

public class GetDeactivatedUserNotificationsRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetDeactivatedUserNotificationsValidator();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

public class GetDeactivatedUserNotificationsValidator : AbstractValidator<GetDeactivatedUserNotificationsRequest>
{
    public GetDeactivatedUserNotificationsValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
    }
}
