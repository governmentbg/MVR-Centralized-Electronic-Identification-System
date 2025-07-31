using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

public class GetEmpowermentsByFilterValidator : AbstractValidator<GetEmpowermentsByFilter>
{
    public GetEmpowermentsByFilterValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();

        When(x => x.CreatedOnFrom.HasValue, () =>
        {
            RuleFor(r => r.CreatedOnFrom).LessThanOrEqualTo(DateTime.UtcNow);
        });

        When(x => x.CreatedOnTo.HasValue, () =>
        {
            RuleFor(r => r.CreatedOnTo).LessThan(DateTime.UtcNow.Date.AddDays(1));
        });

        RuleFor(r => r.Status).IsInEnum();
        RuleFor(r => r.SortBy).IsInEnum();
        When(r => r.SortBy.HasValue, () =>
        {
            RuleFor(r => r.SortDirection).IsInEnum();
        });
        When(r => r.EmpoweredUids is not null, () =>
        {
            RuleFor(r => r.EmpoweredUids)
                .NotEmpty()
                .ForEach(r => r.SetValidator(new UserIdentifierValidator()));
        });
        When(r => r.ValidToDate.HasValue, () =>
        {
            RuleFor(r => r.ShowOnlyNoExpiryDate).NotEqual(true);
        });
        When(r => r.ShowOnlyNoExpiryDate.HasValue && r.ShowOnlyNoExpiryDate.Value == true, () =>
        {
            RuleFor(r => r.ValidToDate).Equals(null);
        });
        When(r => r.OnBehalfOf.HasValue, () =>
        {
            RuleFor(r => r.OnBehalfOf).IsInEnum();
        });

        When(r => !string.IsNullOrWhiteSpace(r.EmpowermentUid), () =>
        {
            RuleFor(x => x.EmpowermentUid)
                .Cascade(CascadeMode.Stop)
                .Must(x => ValidatorHelpers.EikFormatIsValid(x) || ValidatorHelpers.UidFormatIsValid(x))
                    .WithMessage("{PropertyName} contains invalid Uid.");
        });

        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    }
}
