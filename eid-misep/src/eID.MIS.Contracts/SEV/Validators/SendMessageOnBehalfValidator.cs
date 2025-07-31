using eID.MIS.Contracts.SEV.Commands;
using FluentValidation;

namespace eID.MIS.Contracts.SEV.Validators;

public class SendMessageOnBehalfValidator : AbstractValidator<SendMessageOnBehalf>
{
    public SendMessageOnBehalfValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.EIdentityId).NotEmpty();
        RuleFor(r => r.Request).NotNull().SetValidator(new SendMessageOnBehalfRequestValidator());
    }
}
