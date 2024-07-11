using FluentValidation;

namespace eID.PAN.API.Requests;

public class SendDirectEmailRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new SendDirectEmailRequestValidator();
    public string Language { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; } 
    public string EmailAddress { get; set; }
}

public class SendDirectEmailRequestValidator : AbstractValidator<SendDirectEmailRequest>
{
    public SendDirectEmailRequestValidator()
    {
        RuleFor(r => r.Language).NotEmpty();
        RuleFor(r => r.Subject).NotEmpty();
        RuleFor(r => r.Body).NotEmpty();
        RuleFor(r => r.EmailAddress).NotEmpty().EmailAddress();
    }
}

