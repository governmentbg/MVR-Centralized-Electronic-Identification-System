using eID.MIS.Contracts.SEV.External;
using FluentValidation;


namespace eID.MIS.Contracts.SEV.Validators;

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.RecipientProfileIds)
            .NotEmpty();

        When(x => x.RecipientProfileIds != null, () =>
        {
            RuleForEach(x => x.RecipientProfileIds).NotEmpty();
        });

        RuleFor(x => x.Subject)
            .NotEmpty();

        RuleFor(x => x.TemplateId)
            .NotEmpty();
    }
}
