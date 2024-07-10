using FluentValidation;

namespace eID.POD.API.Requests;

public class ManualUploadDatasetRequest : IValidatableRequest
{
    public Guid Id { get; set; }
    public virtual IValidator GetValidator() => new ManualUploadDatasetRequestValidator();
}

public class ManualUploadDatasetRequestValidator : AbstractValidator<ManualUploadDatasetRequest>
{
    public ManualUploadDatasetRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
