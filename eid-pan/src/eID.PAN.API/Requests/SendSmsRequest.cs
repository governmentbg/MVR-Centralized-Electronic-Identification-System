using FluentValidation;

namespace eID.PAN.API.Requests;

public class SendSmsRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new SendSmsRequestValidator();
    public Guid UserId { get; set; }
    public IEnumerable<SendSmsTranslation> Translations { get; set; } = Enumerable.Empty<SendSmsTranslation>();

}

public class SendSmsRequestValidator : AbstractValidator<SendSmsRequest>
{
    public SendSmsRequestValidator()
    {
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t?.Count() >= 2)
            .Must(t => (t?.Any(c => c.Language == "bg") ?? false) && (t?.Any(c => c.Language == "en") ?? false))
                .WithMessage($"{nameof(SendEmailRequest.Translations)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new SendSmsTranslationValidator()));
    }
}

public class SendSmsTranslation : Contracts.Commands.SendSmsTranslation
{
    public string Language { get; set; }
    public string Message { get; set; }
}

public class SendSmsTranslationValidator : AbstractValidator<SendSmsTranslation>
{
    public SendSmsTranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.Message).NotEmpty().MinimumLength(2);
    }
}
