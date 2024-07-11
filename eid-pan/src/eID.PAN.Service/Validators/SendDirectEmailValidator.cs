using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class SendDirectEmailValidator : AbstractValidator<SendDirectEmail>
{
    public SendDirectEmailValidator()
    {
        RuleFor(r => r.Language).NotEmpty();
        RuleFor(r => r.Subject).NotEmpty();
        RuleFor(r => r.Body).NotEmpty();
        RuleFor(r => r.EmailAddress).NotEmpty().EmailAddress();
    }
}
