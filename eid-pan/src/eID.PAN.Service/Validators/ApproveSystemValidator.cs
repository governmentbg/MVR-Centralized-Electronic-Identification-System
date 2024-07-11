using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class ApproveSystemValidator : AbstractValidator<ApproveSystem>
{
    public ApproveSystemValidator()
    {
        RuleFor(r => r.SystemId).NotEmpty();
    }
}
