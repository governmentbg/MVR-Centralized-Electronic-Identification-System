using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

public class GetEmpowermentsFromMeByFilterValidator : AbstractValidator<GetEmpowermentsFromMeByFilter>
{
    public GetEmpowermentsFromMeByFilterValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
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
            RuleFor(r => r.OnBehalfOf).NotNull().IsInEnum();
        });

        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} is below lawful age.");
        RuleFor(r => r.UidType).NotEmpty().IsInEnum();
    }
}
