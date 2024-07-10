using eID.POD.Contracts.Commands;
using FluentValidation;

namespace eID.POD.Service.Validators;

public class DeleteDatasetValidator : AbstractValidator<DeleteDataset>
{
    public DeleteDatasetValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.LastModifiedBy).NotEmpty();
    }
}
