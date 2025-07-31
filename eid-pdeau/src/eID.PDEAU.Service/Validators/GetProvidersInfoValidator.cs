using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

internal class GetProvidersInfoValidator : AbstractValidator<GetProvidersInfo>
{
    public GetProvidersInfoValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
        RuleFor(r => r.SortBy).IsInEnum();
        RuleFor(r => r.SortDirection).IsInEnum();
    }
}
