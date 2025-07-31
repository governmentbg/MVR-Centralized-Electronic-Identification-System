using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class GetAllProvidersRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetAllProvidersRequestValidator();
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}

public class GetAllProvidersRequestValidator : AbstractValidator<GetAllProvidersRequest>
{
    public GetAllProvidersRequestValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
    }
}
