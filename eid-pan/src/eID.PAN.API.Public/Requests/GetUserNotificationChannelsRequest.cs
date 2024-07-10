using FluentValidation;

namespace eID.PAN.API.Public.Requests;

public class GetUserNotificationChannelsRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetUserNotificationChannelsRequestValidator();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string ChannelName { get; set; } = string.Empty;
}

internal class GetUserNotificationChannelsRequestValidator : AbstractValidator<GetUserNotificationChannelsRequest>
{
    public GetUserNotificationChannelsRequestValidator() 
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
    }
}
