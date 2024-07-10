using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class GetSystemByIdValidator : AbstractValidator<GetSystemById>
{
    public GetSystemByIdValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
