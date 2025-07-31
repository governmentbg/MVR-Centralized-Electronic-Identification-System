using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

internal class GetUserAdministratorActionsValidator : AbstractValidator<GetUserAdministratorActions>
{
    public GetUserAdministratorActionsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
    }
}
