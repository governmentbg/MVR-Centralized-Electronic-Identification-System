using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

internal class DeleteUserValidator : AbstractValidator<DeleteUser>
{
    public DeleteUserValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.ProviderId).NotEmpty();
    }
}
