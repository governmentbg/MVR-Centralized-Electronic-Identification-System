using FluentValidation;

namespace eID.PDEAU.API.Public.Requests;

public class GetProviderFileRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetProviderFileRequestValidator();
    public Guid ProviderId { get; set; }
    public Guid FileId { get; set; }
}

public class GetProviderFileRequestValidator : AbstractValidator<GetProviderFileRequest>
{
    public GetProviderFileRequestValidator()
    {
        RuleFor(r => r.FileId).NotEmpty();
    }
}
