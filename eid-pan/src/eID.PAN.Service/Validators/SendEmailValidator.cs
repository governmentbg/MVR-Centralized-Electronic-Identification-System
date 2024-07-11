using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class SendEmailValidator : AbstractValidator<SendEmail>
{
    public SendEmailValidator()
    {
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t?.Count() >= 2)
            .Must(t => (t?.Any(c => c.Language == "bg") ?? false) && (t?.Any(c => c.Language == "en") ?? false))
                .WithMessage($"{nameof(SendEmail.Translations)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new SendEmailTranslationValidator()));
    }
}

internal class SendEmailTranslationValidator : AbstractValidator<SendEmailTranslation>
{
    public SendEmailTranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.Message).NotEmpty().MinimumLength(2);
    }
}
