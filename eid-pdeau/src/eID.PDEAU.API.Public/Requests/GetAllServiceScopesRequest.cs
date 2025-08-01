using FluentValidation;

namespace eID.PDEAU.API.Public.Requests;

public class GetAllServiceScopesRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetAllServiceScopesRequestValidator();
    public Guid ServiceId { get; set; }
}

internal class GetAllServiceScopesRequestValidator : AbstractValidator<GetAllServiceScopesRequest>
{
    public GetAllServiceScopesRequestValidator()
    {
        RuleFor(r => r.ServiceId).NotEmpty();
    }
}
