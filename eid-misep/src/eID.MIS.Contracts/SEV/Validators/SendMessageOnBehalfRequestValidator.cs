using eID.MIS.Contracts.SEV.External;
using FluentValidation;


namespace eID.MIS.Contracts.SEV.Validators;

public class SendMessageOnBehalfRequestValidator : AbstractValidator<SendMessageOnBehalfRequest>
{
    public SendMessageOnBehalfRequestValidator()
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
