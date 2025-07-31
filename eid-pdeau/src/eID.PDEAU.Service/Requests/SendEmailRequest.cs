using FluentValidation;

namespace eID.PDEAU.Service.Requests;

public class SendEmailRequest
{
    public Guid CorrelationId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

internal class SendEmailRequestValidator : AbstractValidator<SendEmailRequest>
{
    public SendEmailRequestValidator()
    {
        RuleFor(r => r.CorrelationId)
            .NotEmpty();

        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(r => r.Subject)
            .NotEmpty();

        RuleFor(r => r.Language)
            .NotEmpty()
            .Length(2);

        RuleFor(r => r.Body)
            .NotEmpty();
    }
}
