using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class AdministratorRegisterUserRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new AdministratorRegisterUserRequestValidator();

    public Guid ProviderId { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsAdministrator { get; set; }
    public string Comment { get; set; }
}

internal class AdministratorRegisterUserRequestValidator : AbstractValidator<AdministratorRegisterUserRequest>
{
    public AdministratorRegisterUserRequestValidator()
    {
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.Uid)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid EGN.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid)).When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
            .WithMessage("{PropertyName} is below lawful age.")
            .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.UidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid LNCh.");

        RuleFor(r => r.UidType).NotEmpty().IsInEnum();
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Comment).NotEmpty().MaximumLength(Constants.AdministratorAction.CommentMaxLength);
    }
}
