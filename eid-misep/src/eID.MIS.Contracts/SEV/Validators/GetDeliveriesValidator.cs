using eID.MIS.Contracts.SEV.Commands;
using FluentValidation;

namespace eID.MIS.Contracts.SEV.Validators;

public class GetDeliveriesValidator : AbstractValidator<GetDeliveries>
{
    public GetDeliveriesValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.EIdentityId).NotEmpty();
    }
}
