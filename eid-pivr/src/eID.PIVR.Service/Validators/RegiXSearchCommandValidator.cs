using eID.PIVR.Contracts.Commands;
using FluentValidation;

namespace eID.PIVR.Service.Validators
{
    public class RegiXSearchCommandValidator : AbstractValidator<RegiXSearchCommand>
    {
        public RegiXSearchCommandValidator()
        {
            RuleFor(r => r.CorrelationId).NotEmpty();
            RuleFor(r => r.Command).NotEmpty();
        }
    }
}
