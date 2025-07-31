using eID.PIVR.Contracts.Enums;
using FluentValidation;

namespace eID.PIVR.API.Requests;

public class CheckUidRestrictionsQuery : IValidatableRequest
{
    public virtual IValidator GetValidator() => new CheckUidRestrictionsQueryValidator();
    public string Uid { get; set; }
    public UidType UidType { get; set; }
}

internal class CheckUidRestrictionsQueryValidator : AbstractValidator<CheckUidRestrictionsQuery>
{
    public CheckUidRestrictionsQueryValidator()
    {
        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.");

        RuleFor(r => r.UidType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .IsInEnum();
    }
}
