using FluentValidation;

namespace eID.PAN.API.Requests;

public class SendEmailRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new SendEmailRequestValidator();
    public Guid UserId { get; set; }
    public IEnumerable<SendEmailTranslation> Translations { get; set; } = Enumerable.Empty<SendEmailTranslation>();

}

public class SendEmailRequestValidator : AbstractValidator<SendEmailRequest>
{
    public SendEmailRequestValidator()
    {
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t?.Count() >= 2)
            .Must(t => (t?.Any(c => c.Language == "bg") ?? false) && (t?.Any(c => c.Language == "en") ?? false))
                .WithMessage($"{nameof(SendEmailRequest.Translations)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new SendEmailTranslationValidator()));
    }
}

public class SendEmailTranslation : Contracts.Commands.SendEmailTranslation
{
    public string Language { get; set; }
    public string Message { get; set; }
}

internal class SendEmailTranslationValidator : AbstractValidator<SendEmailTranslation>
{
    public SendEmailTranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.Message).NotEmpty().MinimumLength(2);
    }
}
