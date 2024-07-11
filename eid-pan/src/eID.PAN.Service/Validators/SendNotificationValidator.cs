using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

internal class SendNotificationValidator : AbstractValidator<SendNotification>
{
    public SendNotificationValidator()
    {
        RuleFor(r => r.SystemName).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.EventCode).NotEmpty().MinimumLength(2).MaximumLength(64);

        When(r => string.IsNullOrWhiteSpace(r.Uid) && r.EId is null, () =>
        {
            RuleFor(r => r.UserId)
            .NotEmpty()
            .WithMessage("At least one of UserId, Uid or EId is required");
        });

        When(r => !string.IsNullOrWhiteSpace(r.Uid) || r.EId is not null, () =>
        {
            RuleFor(r => r.UserId)
                .Empty()
                .WithMessage("Only one of UserId, Uid or EId is allowed");
        });

        When(r => !string.IsNullOrWhiteSpace(r.Uid) || r.UserId is not null, () =>
        {
            RuleFor(r => r.EId)
                .Empty()
                .WithMessage("Only one of UserId, Uid or EId is allowed");
        });

        When(r => r.EId is not null || r.UserId is not null, () =>
        {
            RuleFor(r => r.Uid)
                .Empty()
                .WithMessage("Only one of UserId, Uid or EId is allowed");
        });

        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t?.Count() >= 2)
            .Must(t => (t?.Any(c => c.Language == "bg") ?? false) && (t?.Any(c => c.Language == "en") ?? false))
                .WithMessage($"{nameof(SendNotification.Translations)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new SendNotificationTranslationValidator()));

        When(r => !string.IsNullOrWhiteSpace(r.Uid), () =>
        {
            RuleFor(r => r.UidType)
                .NotEmpty()
                .IsInEnum()
                .WithMessage($"Please specify {nameof(SendNotification.UidType)}");
        });
    }
}

internal class SendNotificationTranslationValidator : AbstractValidator<SendNotificationTranslation>
{
    public SendNotificationTranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.Message).NotEmpty().MinimumLength(2);
    }
}
