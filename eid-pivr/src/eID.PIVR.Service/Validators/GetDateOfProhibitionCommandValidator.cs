using eID.PIVR.Contracts.Commands;
using FluentValidation;

namespace eID.PIVR.Service.Validators;

public class GetDateOfProhibitionCommandValidator : AbstractValidator<GetDateOfProhibition>
{
    public GetDateOfProhibitionCommandValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.PersonalId).NotEmpty().Matches("^\\d+$").MaximumLength(10);
    }
}
