using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class GetUserAdministratorActionsRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetUserAdministratorActionsRequestValidator();
    public Guid ProviderId { get; set; }
    public Guid UserId { get; set; }
}

internal class GetUserAdministratorActionsRequestValidator : AbstractValidator<GetUserAdministratorActionsRequest>
{
    public GetUserAdministratorActionsRequestValidator()
    {
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
    }
}

