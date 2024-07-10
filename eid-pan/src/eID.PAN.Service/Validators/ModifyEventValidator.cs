using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class ModifyEventValidator : AbstractValidator<ModifyEvent>
{
    public ModifyEventValidator()
    {
    }
}
