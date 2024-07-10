using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class RegisterNotificationChannelValidator : AbstractValidator<RegisterNotificationChannel>
{
    public RegisterNotificationChannelValidator()
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
            .ForEach(r => r.SetValidator(new NotificationChannelTranslationValidator()));
    }
}

public class NotificationChannelTranslationValidator : AbstractValidator<NotificationChannelTranslation>
{
    public NotificationChannelTranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.Name).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.Description).NotEmpty().MinimumLength(2).MaximumLength(128);
    }
}
