using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class ModifyNotificationChannelValidator : AbstractValidator<ModifyNotificationChannel>
{
    public ModifyNotificationChannelValidator()
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
