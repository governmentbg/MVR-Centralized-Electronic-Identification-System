using eID.PUN.Contracts.Commands;
using FluentValidation;

namespace eID.PUN.Service.Validators;

public class NotifyEIdsValidator : AbstractValidator<NotifyEIds>
{
    public NotifyEIdsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.EIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .ForEach(r => r.NotNull().NotEmpty());
        RuleFor(r => r.EventCode).NotEmpty();
        RuleFor(r => r.Translations)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t.Count() >= 2)
            .Must(t => t.Any(c => c.Language == "bg") && t.Any(c => c.Language == "en"))
                .WithMessage($"{nameof(Translation)} languages must contain at least 'bg' and 'en' records.")
            .ForEach(r => r.SetValidator(new TranslationValidator()));
    }
}

public class TranslationValidator : AbstractValidator<Translation>
{
    public TranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.ShortDescription).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.Description).NotEmpty().MinimumLength(2).MaximumLength(128);
    }
}
