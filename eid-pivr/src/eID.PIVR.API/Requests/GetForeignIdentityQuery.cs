using eID.PIVR.Contracts.Enums;
using FluentValidation;

namespace eID.PIVR.API.Requests;

public class GetForeignIdentityQuery : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetForeignIdentityQueryValidator();

    public IdentifierType IdentifierType { get; set; }
    public string Identifier { get; set; } = string.Empty;
}

internal class GetForeignIdentityQueryValidator : AbstractValidator<GetForeignIdentityQuery>
{
    public GetForeignIdentityQueryValidator()
    {
        RuleFor(r => r.Identifier)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.");

        RuleFor(r => r.IdentifierType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .IsInEnum();
    }
}
