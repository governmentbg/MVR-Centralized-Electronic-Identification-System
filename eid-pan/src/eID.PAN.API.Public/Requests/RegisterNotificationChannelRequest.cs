using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.API.Public.Requests;

public class RegisterNotificationChannelRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RegisterNotificationChannelRequestValidator();
    public string SystemId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CallbackUrl { get; set; }
    public decimal Price { get; set; }
    public string InfoUrl { get; set; }
    public IEnumerable<NotificationChannelTranslationRequest> Translations { get; set; } = Enumerable.Empty<NotificationChannelTranslationRequest>();
}

public class RegisterNotificationChannelRequestValidator : AbstractValidator<RegisterNotificationChannelRequest>
{
    public RegisterNotificationChannelRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.CallbackUrl).NotEmpty();
        RuleFor(r => r.InfoUrl).NotEmpty();
        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t.Count() >= 2)
            .Must(t => t.Any(c => c.Language == "bg") && t.Any(c => c.Language == "en"))
                .WithMessage($"{nameof(Translation)} languages must contain at least 'bg' and 'en' records.")
            .ForEach(r => r.SetValidator(new NotificationChannelTranslationRequestValidator()));
    }
}

public class NotificationChannelTranslationRequest : NotificationChannelTranslation
{
    public string Language { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class NotificationChannelTranslationRequestValidator : AbstractValidator<NotificationChannelTranslationRequest>
{
    public NotificationChannelTranslationRequestValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.Name).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.Description).NotEmpty().MinimumLength(2).MaximumLength(128);
    }
}
