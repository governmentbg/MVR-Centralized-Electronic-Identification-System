using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.API.Requests;

public class RegisterSystemRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RegisterSystemRequestValidator();
    public IEnumerable<RegisteredSystemTranslationRequest> Translations { get; set; } = Enumerable.Empty<RegisteredSystemTranslationRequest>();
    public IEnumerable<SystemEventRequest> Events { get; set; } = Enumerable.Empty<SystemEventRequest>();
}

public class RegisterSystemRequestValidator : AbstractValidator<RegisterSystemRequest>
{
    public RegisterSystemRequestValidator() 
    {
        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t.Count() >= 2)
            .Must(t => t.Any(c => c.Language == "bg") && t.Any(c => c.Language == "en"))
                .WithMessage($"{nameof(RegisterSystem.Translations)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new RegisteredSystemTranslationRequestValidator()));
        RuleFor(r => r.Events)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t.Any())
            .Must(e => e.Select(c => c.Code.ToLower()).Distinct().Count() == e.Count())
                .WithMessage($"Some of {nameof(SystemEvent)}.{nameof(SystemEvent.Code)}(s) are duplicated")
            .ForEach(r => r.SetValidator(new SystemEventRequestValidator()));
    }
}

public class SystemEventRequest: SystemEvent
{
    public string Code { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }

    public IEnumerable<TranslationRequest> Translations { get; set; } = Enumerable.Empty<TranslationRequest>();

    IEnumerable<Translation> SystemEvent.Translations { get => Translations; set => throw new NotImplementedException(); }
}

public class SystemEventRequestValidator : AbstractValidator<SystemEventRequest>
{
    public SystemEventRequestValidator()
    {
        RuleFor(r => r.Code).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t.Count() >= 2)
            .Must(t => t.Any(c => c.Language == "bg") && t.Any(c => c.Language == "en"))
                .WithMessage($"{nameof(Translation)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new TranslationRequestValidator()));
    }
}

public class TranslationRequest: Translation
{
    public string Language { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class TranslationRequestValidator : AbstractValidator<TranslationRequest>
{
    public TranslationRequestValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.ShortDescription).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.Description).NotEmpty().MinimumLength(2).MaximumLength(128);
    }
}

public class RegisteredSystemTranslationRequest: RegisteredSystemTranslation
{
    public string Language { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
public class RegisteredSystemTranslationRequestValidator : AbstractValidator<RegisteredSystemTranslationRequest>
{
    public RegisteredSystemTranslationRequestValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.Name).NotEmpty().MinimumLength(2).MaximumLength(128);
    }
}
