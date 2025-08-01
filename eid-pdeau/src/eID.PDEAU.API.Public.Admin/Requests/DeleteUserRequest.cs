using FluentValidation;

namespace eID.PDEAU.API.Public.Admin.Requests;

public class DeleteUserRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new DeleteUserRequestValidator();
    public Guid ProviderId { get; set; }
    public Guid Id { get; set; }
}

internal class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
{
    public DeleteUserRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.ProviderId).NotEmpty();
    }
}

