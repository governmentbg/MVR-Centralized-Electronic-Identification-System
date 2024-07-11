using FluentValidation;

namespace eID.POD.API.Requests;

public class DeleteDatasetRequest : IValidatableRequest
{
    public Guid Id { get; set; }
   
    public virtual IValidator GetValidator() => new DeleteDatasetRequestValidator();
}

public class DeleteDatasetRequestValidator : AbstractValidator<DeleteDatasetRequest>
{
    public DeleteDatasetRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}

