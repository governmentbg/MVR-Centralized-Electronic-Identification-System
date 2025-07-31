using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class ApproveServiceValidator : AbstractValidator<ApproveService>
{
    public ApproveServiceValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.ServiceId).NotEmpty();
    }
}
