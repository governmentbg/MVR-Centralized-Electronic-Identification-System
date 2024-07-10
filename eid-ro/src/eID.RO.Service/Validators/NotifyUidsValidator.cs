using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;
internal class NotifyUidsValidator : AbstractValidator<NotifyUids>
{
    public NotifyUidsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.EmpowermentId).NotEmpty();
        RuleFor(r => r.Uids)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.SetValidator(new UserIdentifierValidator()));
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
