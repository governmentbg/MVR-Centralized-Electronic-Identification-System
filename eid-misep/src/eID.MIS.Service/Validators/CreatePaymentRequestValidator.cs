using eID.MIS.Contracts.EP.Commands;
using eID.MIS.Contracts.EP.Validators;
using FluentValidation;

namespace eID.MIS.Service.Validators;

internal class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Request).NotNull().SetValidator(new RegisterPaymentRequestValidator());
    }
}
