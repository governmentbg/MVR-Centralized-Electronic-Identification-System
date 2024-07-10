using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class SendSmsValidator : AbstractValidator<SendSms>
{
    public SendSmsValidator()
    {
        RuleFor(r => r.UserId).NotEmpty();

        RuleFor(r => r.Translations)
            .NotEmpty();
        When(r => r.Translations != null, () =>
        {
            RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .ForEach(r => r.NotEmpty())
            .Must(t => t.Count() >= 2)
            .Must(t => t.Any(c => c.Language == "bg") && t.Any(c => c.Language == "en"))
                .WithMessage($"{nameof(Translation)} languages must contain at least 'bg' and 'en' records.")
            .ForEach(r => r.SetValidator(new SendSmsTranslationValidator()));
        });
    }
}

public class SendSmsTranslationValidator : AbstractValidator<SendSmsTranslation>
{
    public SendSmsTranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().Length(2);
        // https://firebase.google.com/docs/cloud-messaging/concept-options#notifications_and_data_messages
        // Notification messages contain a predefined set of user-visible keys.
        // Data messages, by contrast, contain only your user-defined custom key-value pairs.
        // Notification messages can contain an optional data payload.
        // Maximum payload for both message types is 4000 bytes, except when sending messages from the Firebase console,
        // which enforces a 1024 character limit.
        RuleFor(r => r.Message).NotEmpty().MaximumLength(1024);
    }
}
