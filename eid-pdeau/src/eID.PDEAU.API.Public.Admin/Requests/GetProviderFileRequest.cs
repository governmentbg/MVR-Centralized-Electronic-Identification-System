using FluentValidation;

namespace eID.PDEAU.API.Public.Admin.Requests;

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
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.FileId).NotEmpty();
    }
}
