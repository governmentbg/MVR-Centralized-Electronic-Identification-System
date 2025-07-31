using eID.MIS.Contracts.EP.Commands;
using FluentValidation;

namespace eID.MIS.Service.Validators;

internal class GetClientsByEikValidator : AbstractValidator<GetClientsByEik>
{
    public GetClientsByEikValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Eik).NotEmpty();
    }
}
