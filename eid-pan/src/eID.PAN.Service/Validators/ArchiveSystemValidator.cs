using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class ArchiveSystemValidator : AbstractValidator<ArchiveSystem>
{
    public ArchiveSystemValidator()
    {
        RuleFor(r => r.SystemId).NotEmpty();
    }
}
