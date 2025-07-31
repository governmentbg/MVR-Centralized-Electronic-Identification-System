using eID.PIVR.Contracts.Commands;
using FluentValidation;

namespace eID.PIVR.Service.Validators;

public class GetDateOfDeathCommandValidator : AbstractValidator<GetDateOfDeath>
{
    public GetDateOfDeathCommandValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.PersonalId).NotEmpty().Matches("^\\d+$").MaximumLength(10);
        RuleFor(r => r.UidType).NotEmpty().IsInEnum();
    }
}
