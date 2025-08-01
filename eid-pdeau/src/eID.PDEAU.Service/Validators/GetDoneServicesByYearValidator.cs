using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

internal class GetDoneServicesByYearValidator : AbstractValidator<GetDoneServicesByYear>
{
    public GetDoneServicesByYearValidator()
    {
        RuleFor(r => r.Year).Cascade(CascadeMode.Stop).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}
