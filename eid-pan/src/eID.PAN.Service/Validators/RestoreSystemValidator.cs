using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class RestoreSystemValidator : AbstractValidator<RestoreSystem>
{
    public RestoreSystemValidator()
    {
        RuleFor(r => r.SystemId).NotEmpty();
    }
}
