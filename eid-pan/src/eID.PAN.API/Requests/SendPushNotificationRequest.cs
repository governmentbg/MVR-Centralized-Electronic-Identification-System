using FluentValidation;

namespace eID.PAN.API.Requests;

public class SendPushNotificationRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new SendPushNotificationRequestValidator();
    public Guid UserId { get; set; }
    public IEnumerable<SendPushNotificationTranslation> Translations { get; set; } = Enumerable.Empty<SendPushNotificationTranslation>();

}

public class SendPushNotificationRequestValidator : AbstractValidator<SendPushNotificationRequest>
{
    public SendPushNotificationRequestValidator()
    {
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t?.Count() >= 2)
            .Must(t => (t?.Any(c => c.Language == "bg") ?? false) && (t?.Any(c => c.Language == "en") ?? false))
                .WithMessage($"{nameof(SendPushNotificationRequest.Translations)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new SendPushNotificationTranslationValidator()));
    }
}

public class SendPushNotificationTranslation : Contracts.Commands.PushNotificationTranslation
{
    public string Language { get; set; }
    public string Message { get; set; }
}

public class SendPushNotificationTranslationValidator : AbstractValidator<SendPushNotificationTranslation>
{
    public SendPushNotificationTranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.Message).NotEmpty().MinimumLength(2);
    }
}
