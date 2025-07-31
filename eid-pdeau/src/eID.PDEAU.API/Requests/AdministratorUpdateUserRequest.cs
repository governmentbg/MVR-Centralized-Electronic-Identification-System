using eID.PDEAU.Contracts;
using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class AdministratorUpdateUserRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new AdministratorUpdateUserRequestValidator();

    public Guid ProviderId { get; set; }

    public Guid Id { get; set; }
    public bool IsAdministrator { get; set; }
    public string Comment { get; set; }
}

internal class AdministratorUpdateUserRequestValidator : AbstractValidator<AdministratorUpdateUserRequest>
{
    public AdministratorUpdateUserRequestValidator()
    {
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.Comment).NotEmpty().MaximumLength(Constants.AdministratorAction.CommentMaxLength);
    }
}
