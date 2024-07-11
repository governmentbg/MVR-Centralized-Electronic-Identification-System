using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

public class GetEmpowermentsToMeByFilterValidator : AbstractValidator<GetEmpowermentsToMeByFilter>
{
    public GetEmpowermentsToMeByFilterValidator()
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
        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} contains invalid records.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} contains records of people below lawful age.");
        RuleFor(r => r.UidType).NotEmpty().IsInEnum();
        When(r => r.ValidToDate.HasValue, () =>
        {
            RuleFor(r => r.ShowOnlyNoExpiryDate).NotEqual(true);
        });
        When(r => r.ShowOnlyNoExpiryDate.HasValue && r.ShowOnlyNoExpiryDate.Value == true, () =>
        {
            RuleFor(r => r.ValidToDate).Equals(null);
        });
    }
}
