using FluentValidation;

namespace eID.PAN.API.Public.Requests;

public class GetSelectedUserNotificationChannelsRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetSelectedUserNotificationChannelsValidator();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

internal class GetSelectedUserNotificationChannelsValidator : AbstractValidator<GetSelectedUserNotificationChannelsRequest>
{
    public GetSelectedUserNotificationChannelsValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
    }
}
