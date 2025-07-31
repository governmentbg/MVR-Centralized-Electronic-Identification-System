using eID.PAN.Contracts;
using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class RejectSystemValidator : AbstractValidator<RejectSystem>
{
    public RejectSystemValidator()
    {
        RuleFor(r => r.CorrelationId)
            .NotEmpty();
        
        RuleFor(r => r.SystemId)
            .NotEmpty();

        RuleFor(r => r.UserId)
            .NotEmpty();

        RuleFor(r => r.Reason)
            .NotEmpty()
            .MaximumLength(FieldLength.RegisteredSystemRejected.RejectReason);
    }
}
