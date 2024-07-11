using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class RejectSystemValidator : AbstractValidator<RejectSystem>
{
    public RejectSystemValidator()
    {
        RuleFor(r => r.SystemId).NotEmpty();
    }
}
