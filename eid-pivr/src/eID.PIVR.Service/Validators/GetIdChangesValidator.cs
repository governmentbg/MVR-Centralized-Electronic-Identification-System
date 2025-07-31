using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Enums;
using FluentValidation;

namespace eID.PIVR.Service.Validators;

public class GetIdChangesValidator : AbstractValidator<GetIdChanges>
{
    public GetIdChangesValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(r => r.CorrelationId).NotEmpty();

        When(r => r.PersonalId != null || r.UidType.HasValue, () =>
        {
            RuleFor(r => r.PersonalId)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.UidType == UidType.EGN, ApplyConditionTo.CurrentValidator)
                    .WithMessage("{PropertyName} invalid EGN.")
                .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.UidType == UidType.LNCh, ApplyConditionTo.CurrentValidator)
                    .WithMessage("{PropertyName} invalid LNCh.");

            RuleFor(x => x.UidType)
                .NotEmpty()
                .NotEqual(UidType.None)
                .IsInEnum();

            When(r => r.CreatedOnFrom.HasValue || r.CreatedOnTo.HasValue, () =>
            {
                // When either one is provided - the other is required and From date must be less than or equal to To date.
                RuleFor(r => r.CreatedOnTo)
                    .NotEmpty();
                RuleFor(r => r.CreatedOnFrom)
                    .NotEmpty()
                    .LessThanOrEqualTo(r => r.CreatedOnTo);
            });
        })
        .Otherwise(() =>
        {
            // Dates are required to avoid dumping the whole DB.
            RuleFor(r => r.CreatedOnTo)
                .NotEmpty();

            RuleFor(r => r.CreatedOnFrom)
                .NotEmpty()
                .LessThanOrEqualTo(r => r.CreatedOnTo);
        });
    }
}
