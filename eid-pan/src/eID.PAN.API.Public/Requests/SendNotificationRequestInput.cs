using eID.PAN.Contracts.Enums;
using FluentValidation;

namespace eID.PAN.API.Public.Requests;

public class SendNotificationRequestInput
{
    public string EventCode { get; set; }
    public Guid? UserId { get; set; }
    public Guid? EId { get; set; }
    public string? Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public IEnumerable<SendNotificationTranslation> Translations { get; set; } = Enumerable.Empty<SendNotificationTranslation>();
}

public class SendNotificationRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new SendNotificationRequestValidator();

    public string SystemName { get; set; }
    public string EventCode { get; set; }
    public Guid? UserId { get; set; }
    public Guid? EId { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public IEnumerable<SendNotificationTranslation> Translations { get; set; } = Enumerable.Empty<SendNotificationTranslation>();
}

internal class SendNotificationRequestValidator : AbstractValidator<SendNotificationRequest>
{
    public SendNotificationRequestValidator()
    {
        RuleFor(r => r.SystemName).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.EventCode).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.UserId)
            .NotEmpty()
            .When(r => string.IsNullOrWhiteSpace(r.Uid) && r.EId is null)
            .WithMessage("At least one of UserId, Uid or EId is required"); ;

        RuleFor(r => r.UserId)
            .Empty()
            .When(r => !string.IsNullOrWhiteSpace(r.Uid) || r.EId is not null)
            .WithMessage("Only one of UserId, Uid or EId is allowed");

        RuleFor(r => r.EId)
            .Empty()
            .When(r => !string.IsNullOrWhiteSpace(r.Uid) || r.UserId is not null)
            .WithMessage("Only one of UserId, Uid or EId is allowed");

        RuleFor(r => r.Uid)
            .Empty()
            .When(r => r.EId is not null || r.UserId is not null)
            .WithMessage("Only one of UserId, Uid or EId is allowed");

        When(r => r.Translations != null && r.Translations.Any(), () =>
        {
            RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .ForEach(r => r.NotEmpty())
            .Must(t => t?.Count() >= 2)
            .Must(t => (t?.Any(c => c.Language == "bg") ?? false) && (t?.Any(c => c.Language == "en") ?? false))
                .WithMessage($"{nameof(SendNotificationRequest.Translations)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new SendNotificationTranslationRequestValidator()));
        });

        When(r => !string.IsNullOrWhiteSpace(r.Uid), () =>
        {
            RuleFor(r => r.UidType)
                .NotEmpty()
                .IsInEnum()
                .WithMessage($"Please specify {nameof(SendNotificationRequest.UidType)}");
        });
    }
}

public class SendNotificationTranslation : Contracts.Commands.SendNotificationTranslation
{
    public string Language { get; set; }
    public string Message { get; set; }
}

internal class SendNotificationTranslationRequestValidator : AbstractValidator<SendNotificationTranslation>
{
    public SendNotificationTranslationRequestValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.Message).NotEmpty().MinimumLength(2);
    }
}
