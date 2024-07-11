using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class RegisterSystemValidator : AbstractValidator<RegisterSystem>
{
    public RegisterSystemValidator()
    {
        RuleFor(r => r.SystemName).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t.Count() >= 2)
            .Must(t => t.Any(c => c.Language == "bg") && t.Any(c => c.Language == "en"))
                .WithMessage($"{nameof(RegisterSystem.Translations)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new RegisteredSystemTranslationValidator()));
        RuleFor(r => r.Events)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(e => e.NotEmpty())
            .Must(e => e.Any())
            .Must(e => e.Select(c => c.Code.ToLower()).Distinct().Count() == e.Count())
                .WithMessage($"Some of {nameof(SystemEvent)}.{nameof(SystemEvent.Code)}(s) are duplicated")
            .ForEach(r => r.SetValidator(new SystemEventValidator()));
    }
}

public class SystemEventValidator : AbstractValidator<SystemEvent>
{
    public SystemEventValidator()
    {
        RuleFor(r => r.Code).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t.Count() >= 2)
            .Must(t => t.Any(c => c.Language == "bg") && t.Any(c => c.Language == "en"))
                .WithMessage($"{nameof(Translation)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new TranslationValidator()));
    }
}

public class TranslationValidator : AbstractValidator<Translation>
{
    public TranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.ShortDescription).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.ShortDescription).NotEmpty().MinimumLength(2).MaximumLength(128);
    }
}

public class RegisteredSystemTranslationValidator : AbstractValidator<RegisteredSystemTranslation>
{
    public RegisteredSystemTranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.Name).NotEmpty().MinimumLength(2).MaximumLength(128);
    }
}
