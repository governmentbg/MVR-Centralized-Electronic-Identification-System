using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class GetByIdRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetByIdRequestValidator();
    public Guid Id { get; set; }
}

internal class GetByIdRequestValidator : AbstractValidator<GetByIdRequest>
{
    public GetByIdRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
